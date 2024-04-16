# MYRIAM
MYRIAM is an open-access, computationally-inexpensive, user-friendly software that allow users to compute the torque variations required to produce given changes in plate motions.


<!-- installation -->
## Installation

### Executable Files

To download a pre-compiled version of MYRIAM, go to the [Releases](https://github.com/ValeEspinozaF/MYRIAM/releases) page and select the latest version. Under the "Assets" section, download the appropriate file for your operating system. Once the download is complete, unzip the file and move the folder to the desired location.

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

See the Manual for an example on how to use MYRIAM to achieve refined results in a time-efficient manner. All necessary files to run the examples are included in the `examples` folder.

You can also find examples of MYRIAM (or the base algorithm) used in plate motions research:
- [Espinoza, V., & Iaffaldano, G. (2023). Rapid absolute plate motion changes inferred from high-resolution relative spreading reconstructions: A case study focusing on the South America plate and its Atlantic/Pacific neighbors. Earth and Planetary Science Letters, 604, 118009.](https://www.sciencedirect.com/science/article/pii/S0012821X23000225)
- [Martin de Blas, J., Iaffaldano, G., Tassara, A., & Melnick, D. (2023). Feedback between megathrust earthquake cycle and plate convergence. Scientific Reports, 13(1), 18623.0](https://www.nature.com/articles/s41598-023-45753-5)
- [Iaffaldano, G., de Blas, J. M., & Udbø, B. Í. D. (2022). Decadal change of the Apulia microplate motion preceding the MW 6.4, 26 November 2019 Durrës (Albania) earthquake. Earth and Planetary Science Letters, 584, 117505.](https://www.sciencedirect.com/science/article/pii/S0012821X22001418)
- [Martin de Blas, J., Iaffaldano, G., & Calais, E. (2022). Have the 1999 Izmit–Düzce earthquakes influenced the motion and seismicity of the Anatolian microplate?. Geophysical Journal International, 229(3), 1754-1769.](https://academic.oup.com/gji/article/229/3/1754/6512137?login=true)


<!-- citing -->
## Citing

Valentina Espinoza, Juan Martin de Blas, & Giampiero Iaffaldano. (2023). ValeEspinozaF/MYRIAM: MYRIAM v1.0.1 (v1.0.1). Zenodo. https://doi.org/10.5281/zenodo.8047062