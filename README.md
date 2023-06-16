# MYRIAM
MYRIAM is an open-access, computationally-inexpensive, user-friendly software that allow users to compute the torque variations required to produce given changes in plate motions.


<!-- installation -->
## Installation

### Source code
#### Pull from repository 

To download the latest version of MYRIAM from a specific repository branch (e.g. `main`), copy the following commands into your terminal:

```sh
cd /path/to/desired/directory       # Directory where to clone MYRIAM
git clone https://github.com/ValeEspinozaF/MYRIAM.git
cd MYRIAM
git checkout main                   # or whichever branch is preferred
git pull                            # fetch all recent changes from this branch
```


#### Download ZIP from GitHub

Alternatively, you can download the latest version of MYRIAM as a ZIP file from the [GitHub repository](https://github.com/ValeEspinozaF/MYRIAM). Under the repository name, click on the blue button that says "Code" and select "Download ZIP". Once the download is complete, unzip the file and move the folder to the desired location.

### Executable Files

To download a pre-compiled version of MYRIAM, go to the [Releases](https://github.com/ValeEspinozaF/MYRIAM/releases) page and select the latest version. Under the "Assets" section, download the appropriate file for your operating system. Once the download is complete, unzip the file and move the folder to the desired location.



<!-- dependencies -->
## Dependencies

MYRIAM calculations run as a self-contained release, and require no additional dependencies. However, to generate PNG figures you will need Python and the following packages:

- [Python 3.8](https://www.python.org/) or later
- [Pandas 1.4.4](https://https://pandas.pydata.org/) or later
- [Matplotlib 3.5](https://matplotlib.org/stable/users/installing/index.html) or later
- [Cartopy 0.18.0](https://scitools.org.uk/cartopy/docs/latest/index.html#getting-started) or later, 
- and dependencies therein*

*Note that Cartopy up to version 0.21.0 may encounter package issues when importing, particularly *lgeos*. Make sure to install/downgrade Shapely to its version 1.8.5 to avoid the above issue.

<!-- examples -->
## Examples


<!-- citing -->
## Citing