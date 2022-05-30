import os
import cv2
import multiprocessing as mp
from multiprocessing.pool import ThreadPool as Pool
import numpy as np
import torch
import matplotlib.pyplot as plt
import torch.nn as nn
import torchvision
import torch.nn.functional as F
import torch.optim as optim
from torch.autograd import Variable
import json

def read_alphabets(alphabet_directory_path, alphabet_directory_name):
    """
    Reads all the characters from a given alphabet_directoryA
    """
    datax = []
    datay = []

    images = os.listdir(alphabet_directory_path)
    for img in images:
        # print(alphabet_directory_path + img)
        image = cv2.resize(cv2.imread(alphabet_directory_path + img), (105, 105))

        # rotations of image

        datax.extend((image))
        datay.extend((
            alphabet_directory_name,

        ))

    return np.array(datax), np.array(datay)

def read_images(base_directory):
    """
    Reads all the alphabets from the base_directory
    Uses multithreading to decrease the reading time drastically
    """
    datax = None
    datay = None
    pool = Pool(mp.cpu_count())
    results = [pool.apply(read_alphabets,
                          args=(
                              base_directory + '/' + directory + '/', directory,
                              )) for directory in os.listdir(base_directory)]
    pool.close()
    for result in results:
        if datax is None:
            datax = result[0]
            datay = result[1]
        else:
            datax = np.vstack([datax, result[0]])
            datay = np.concatenate([datay, result[1]])
    return datax, datay

def extract_sample(n_way, n_support, n_query, datax, datay):
  """
  Picks random sample of size n_support+n_querry, for n_way classes
  Args:
      n_way (int): number of classes in a classification task
      n_support (int): number of labeled examples per class in the support set
      n_query (int): number of labeled examples per class in the query set
      datax (np.array): dataset of images
      datay (np.array): dataset of labels
  Returns:
      (dict) of:
        (torch.Tensor): sample of images. Size (n_way, n_support+n_query, (dim))
        (int): n_way
        (int): n_support
        (int): n_query
  """
  sample = []
  K = np.random.choice(np.unique(datay), n_way, replace=False)

  with open('C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/class_names.txt', 'w') as f:
      for class_names in K:
          f.writelines(str(class_names) + "\n")
      f.close()

  for cls in K:
    datax_cls = datax[datay == cls]
    perm = np.random.permutation(datax_cls)
    sample_cls = perm[:(n_support)]
    sample.append(sample_cls)
  sample = np.array(sample)
  sample = torch.from_numpy(sample).float()
  sample = sample.permute(0, 1, 4, 2, 3)
  return({
      'images': sample,
      'class_names': K,
      'n_way': n_way,
      'n_support': n_support,
      'n_query': n_query
      })

def display_sample(sample):
  """
  Displays sample in a grid
  Args:
      sample (torch.Tensor): sample of images to display
  """
  #need 4D tensor to create grid, currently 5D
  sample_4D = sample.view(sample.shape[0]*sample.shape[1],*sample.shape[2:])
  #make a grid
  out = torchvision.utils.make_grid(sample_4D, nrow=sample.shape[1])
  plt.figure(figsize = (16,7))
  plt.imshow(out.permute(1, 2, 0))


class Flatten(nn.Module):
    def __init__(self):
        super(Flatten, self).__init__()

    def forward(self, x):
        return x.view(x.size(0), -1)


def load_protonet_conv(**kwargs):
    """
    Loads the prototypical network model
    Arg:
        x_dim (tuple): dimension of input image
        hid_dim (int): dimension of hidden layers in conv blocks
        z_dim (int): dimension of embedded image
    Returns:
        Model (Class ProtoNet)
    """
    x_dim = kwargs['x_dim']
    hid_dim = kwargs['hid_dim']
    z_dim = kwargs['z_dim']

    def conv_block(in_channels, out_channels):
        return nn.Sequential(
            nn.Conv2d(in_channels, out_channels, 3, padding=1),
            nn.BatchNorm2d(out_channels),
            nn.ReLU(),
            nn.MaxPool2d(2)
        )

    encoder = nn.Sequential(
        conv_block(x_dim[0], hid_dim),
        conv_block(hid_dim, hid_dim),
        conv_block(hid_dim, hid_dim),
        conv_block(hid_dim, z_dim),
        Flatten()
    )

    return ProtoNet(encoder)


class ProtoNet(nn.Module):
    def __init__(self, encoder):
        """
        Args:
            encoder : CNN encoding the images in sample
            n_way (int): number of classes in a classification task
            n_support (int): number of labeled examples per class in the support set
            n_query (int): number of labeled examples per class in the query set
        """
        super(ProtoNet, self).__init__()
        self.encoder = encoder.cuda()

    def set_forward_loss(self, support_set_sample, query_set_sample):
        """
        Computes loss, accuracy and output for classification task
        Args:
            sample (torch.Tensor): shape (n_way, n_support+n_query, (dim))
        Returns:
            torch.Tensor: shape(2), loss, accuracy and y_hat
        """
        support_sample_images = support_set_sample['images'].cuda()
        query_sample_images = query_set_sample['images'].cuda()
        n_way = support_set_sample['n_way']
        n_support = support_set_sample['n_support']
        n_query = support_set_sample['n_query']

        x_support = support_sample_images
        x_query = query_sample_images

        # target indices are 0 ... n_way-1
        target_inds = torch.arange(0, n_way).view(n_way, 1, 1).expand(n_way, n_query, 1).long()
        target_inds = Variable(target_inds, requires_grad=False)
        target_inds = target_inds.cuda()

        # encode images of the support and the query set
        x = torch.cat([x_support.contiguous().view(n_way * n_support, *x_support.size()[2:]),
                       x_query.contiguous().view(n_way * n_query, *x_query.size()[2:])], 0)

        z = self.encoder.forward(x)
        z_dim = z.size(-1)  # usually 64
        z_proto = z[:n_way * n_support].view(n_way, n_support, z_dim).mean(1)
        z_query = z[n_way * n_support:]

        # compute distances
        dists = euclidean_dist(z_query, z_proto)

        # compute probabilities
        log_p_y = F.log_softmax(-dists, dim=1).view(n_way, n_query, -1)

        loss_val = -log_p_y.gather(2, target_inds).squeeze().view(-1).mean()
        _, y_hat = log_p_y.max(2)
        acc_val = torch.eq(y_hat, target_inds.squeeze()).float().mean()

        return loss_val, {
            'dists': dists,
            'loss': loss_val.item(),
            'acc': acc_val.item(),
            'y_hat': y_hat
        }

def euclidean_dist(x, y):
  """
  Computes euclidean distance btw x and y
  Args:
      x (torch.Tensor): shape (n, d). n usually n_way*n_query
      y (torch.Tensor): shape (m, d). m usually n_way
  Returns:
      torch.Tensor: shape(n, m). For each query, the distances to each centroid
  """
  n = x.size(0)
  m = y.size(0)
  d = x.size(1)
  assert d == y.size(1)

  x = x.unsqueeze(1).expand(n, m, d)
  y = y.unsqueeze(0).expand(n, m, d)

  return torch.pow(x - y, 2).sum(2)

def write_results_to_txt(my_output, my_sample_support):
    with open('C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/estimated_distances.txt', 'w') as f:
        for distance in my_output['dists'][0].tolist():
            f.writelines(str(distance) + "\n")
        f.close()

    with open('C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/estimated_class.txt', 'w') as f:
        f.write(my_sample_support['class_names'][int(my_output['y_hat'][2])])
        f.close()

if __name__ == '__main__':

    support_set_x, support_set_y = read_images('C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/Images/SupportSet')
    query_set_x, query_set_y = read_images('C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/Images/QuerySet')

    support_set_x = support_set_x.reshape(10, 105, 105, 3)
    query_set_x = query_set_x.reshape(5, 105, 105, 3)

    n_way = 1
    n_support = 5
    n_query = 1

    my_sample_query = extract_sample(5, 1, 1, query_set_x, query_set_y)
    my_sample_support = extract_sample(5, 2, 1, support_set_x, support_set_y)

    f = open('C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/ModelConfigurations.json')
    modelsPaths = "C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/Models/"
    data = json.load(f)
    f.close()

    model = load_protonet_conv(
        x_dim=(3, 28, 28),
        hid_dim=64,
        z_dim=64,
    )

    model_path = data['ImageSize'] + '/' + str(data['NumberOfClasses']) + '_way/' + str(data['SupportSet']) + '_support/' + 'sign_model.pt'
    model.load_state_dict(torch.load("C:/Users/erdik/AppData/Roaming/SignatureVerificationSystem/" + model_path))
    model.eval()

    my_loss, my_output = model.set_forward_loss(my_sample_support, my_sample_query)
    write_results_to_txt(my_output, my_sample_support)
