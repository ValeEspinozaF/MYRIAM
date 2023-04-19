# -*- coding: utf-8 -*-
"""
Created on Wed Sep  7 15:26:31 2022

@author: nbt571
"""

# Load modules
import os
import sys
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from matplotlib import cm
import matplotlib.colors as mcolors
from mpl_toolkits.axes_grid1 import make_axes_locatable
from MapFeatures import setCartographic_AxisLabels


# Cmd inputs
modelLabel = sys.argv[1]
boundaryLabel = sys.argv[2]
repositoryDir = sys.argv[3]



# Load grids 
lon_fileName = "GRID_LON_%s.txt" %modelLabel
lat_fileName = "GRID_LAT_%s.txt" %modelLabel
muA_fileName = "GRID_MuA_%s.txt" %modelLabel
ym_fileName = "GRID_YM_%s.txt" %modelLabel
mt_fileName = "GRID_MT_%s.txt" %modelLabel


lon_path = os.path.join(repositoryDir, lon_fileName)
lat_path = os.path.join(repositoryDir, lat_fileName)
muA_path = os.path.join(repositoryDir, muA_fileName)
ym_path = os.path.join(repositoryDir, ym_fileName)
mt_path = os.path.join(repositoryDir, mt_fileName)


lon = pd.read_csv(lon_path, delimiter=' ', header=None)
lat = pd.read_csv(lat_path, delimiter=' ', header=None)
muA = pd.read_csv(muA_path, delimiter=' ', header=None)
ym = pd.read_csv(ym_path, delimiter=' ', header=None)
mt = pd.read_csv(mt_path, delimiter=' ', header=None)


# Load plate contour
contourName = "BDR_%s.txt" %boundaryLabel
contourPath = os.path.join(repositoryDir, contourName)
contourXY = pd.read_csv(contourPath, delimiter=' ', names=["lon", "lat"])


# Load inContour points
inContourName = "BDRin_%s.txt" %boundaryLabel
inContourPath = os.path.join(repositoryDir, inContourName)
inContourXYz = pd.read_csv(inContourPath, delimiter=' ', names=["lon", "lat", "z"])



# --- General setups

# Set Z extent
extent = lon[0][0] - 1, lon.iloc[-1].iloc[-1] + 1, lat[0].iloc[-1] - 1, lat[0][0] + 1

# Create grid colormap
scale_greys = cm.Greys_r(np.linspace(1, 0., 50))
cmap_greys = mcolors.LinearSegmentedColormap.from_list('buffer_cmap', scale_greys)


# =================== Viscosity Map ===================

# Set figure
fig = plt.figure(figsize=(10,7), dpi=360 )
ax = fig.add_subplot(111)
plt.title("Depth-average asthenosphere viscosity [Pa$\cdot$s]", pad=10)


# The width of cax will be 5% of ax and the padding between cax and ax will be fixed at 0.05 inch.
divider = make_axes_locatable(ax)
cax = divider.append_axes("right", size="4%", pad=0.1)


# Set log of muA as Z value
muA_reversed = muA.iloc[::-1]
Z = np.log10(muA_reversed)


# Create the colormap
cmap_viridis = cm.viridis(np.linspace(0., 1, 230))
cmap_viridis1 = cmap_viridis[:50]
cmap_viridis2 = cmap_viridis[90:140]
cmap_viridis3 = cmap_viridis[180:]

colors = np.vstack((cmap_viridis1, cmap_viridis2, cmap_viridis3))
mymap = mcolors.LinearSegmentedColormap.from_list('muA_cmap', colors)


# Plot Z surface colormap
im = ax.imshow(Z, origin='lower', extent=extent, cmap=mymap, vmin=18, vmax=21)
cbar = fig.colorbar(im, cax=cax)
cbar.set_label("$\mathrm{log(\mu_A)}$", rotation=270, fontsize=11, labelpad=25)


# Enclose map to plate's contour 
ax.set_ylim(np.min(contourXY["lat"])-5 , np.max(contourXY["lat"])+5)
ax.set_xlim(np.min(contourXY["lon"])-5 , np.max(contourXY["lon"])+5)


# Plot plate contour and inContour points
ax.plot(contourXY["lon"], contourXY["lat"], '-k', lw=0.7)
scatter = ax.scatter(inContourXYz["lon"], inContourXYz["lat"], s=0.7, 
                     c=inContourXYz["z"], cmap = cmap_greys, vmin=0)


legend1 = ax.legend(*scatter.legend_elements(num=5), loc="upper right", 
                    title="Rigidity", fontsize=8, title_fontsize=10)
ax.add_artist(legend1)


# Set Labels
setCartographic_AxisLabels(ax)


# Save figure as png
figName = "MAP_muA_%s.png" %modelLabel
figpath = os.path.join(repositoryDir, figName)       
plt.savefig(figpath, bbox_inches='tight', dpi=360)




# =================== Young's modulus Map ===================

# Set figure
fig = plt.figure(figsize=(10,7), dpi=360 )
ax = fig.add_subplot(111)
plt.title("Depth-average asthenosphere Young's modulus [Pa]", pad=10)


# The width of cax will be 5% of ax and the padding between cax and ax will be fixed at 0.05 inch.
divider = make_axes_locatable(ax)
cax = divider.append_axes("right", size="4%", pad=0.1)


# Set log of muA as Z value
ym_reversed = ym.iloc[::-1]
Z = ym_reversed


# Create the colormap
cool = cm.get_cmap("cool")
colors = cool(np.linspace(0, 1, 256))
mymap = mcolors.LinearSegmentedColormap.from_list('ym_cmap', colors)


# Plot Z surface colormap
im = ax.imshow(Z, origin='lower', extent=extent, cmap=mymap)
fig.colorbar(im, cax=cax)


# Enclose map to plate's contour 
ax.set_ylim(np.min(contourXY["lat"])-5 , np.max(contourXY["lat"])+5)
ax.set_xlim(np.min(contourXY["lon"])-5 , np.max(contourXY["lon"])+5)


# Plot plate contour and inContour points
ax.plot(contourXY["lon"], contourXY["lat"], '-k', lw=0.8)
scatter = ax.scatter(inContourXYz["lon"], inContourXYz["lat"], s=0.7, 
                     c=inContourXYz["z"], cmap = cmap_greys, vmin=0)


legend1 = ax.legend(*scatter.legend_elements(num=5), loc="upper right", 
                    title="Rigidity", fontsize=8, title_fontsize=10)
ax.add_artist(legend1)


# Set Labels
setCartographic_AxisLabels(ax)


# Save figure as png
figName = "MAP_YM_%s.png" %modelLabel
figpath = os.path.join(repositoryDir, figName)           
plt.savefig(figpath, bbox_inches='tight', dpi=360)




# =================== Maxwel Time Map ===================

# Set figure
fig = plt.figure(figsize=(10,7), dpi=360 )
ax = fig.add_subplot(111)
plt.title("Depth-average asthenosphere Maxwel time [yr]", pad=10)


# The width of cax will be 5% of ax and the padding between cax and ax will be fixed at 0.05 inch.
divider = make_axes_locatable(ax)
cax = divider.append_axes("right", size="4%", pad=0.1)


# Set log of muA as Z value
TAU_MXWyr_reversed = mt.iloc[::-1]
Z = TAU_MXWyr_reversed


# Create the colormap
cool = cm.get_cmap("jet")
colors = cool(np.linspace(1, 0., 100))
mymap = mcolors.LinearSegmentedColormap.from_list('mxw_cmap', colors)


# Plot Z surface colormap
im = ax.imshow(Z, origin='lower', extent=extent, cmap=mymap, vmax=10)
fig.colorbar(im, cax=cax)


# Enclose map to plate's contour 
ax.set_ylim(np.min(contourXY["lat"])-5 , np.max(contourXY["lat"])+5)
ax.set_xlim(np.min(contourXY["lon"])-5 , np.max(contourXY["lon"])+5)


# Plot plate contour and inContour points
ax.plot(contourXY["lon"], contourXY["lat"], '-k', lw=0.8)
scatter = ax.scatter(inContourXYz["lon"], inContourXYz["lat"], s=0.7, 
                     c=inContourXYz["z"], cmap = cmap_greys, vmin=0)


legend1 = ax.legend(*scatter.legend_elements(num=5), loc="upper right", 
                    title="Rigidity", fontsize=8, title_fontsize=10)
ax.add_artist(legend1)


# Set Labels
setCartographic_AxisLabels(ax)


# Save figure as png
figName = "MAP_Mtau_%s.png" %modelLabel
figpath = os.path.join(repositoryDir, figName)           
plt.savefig(figpath, bbox_inches='tight', dpi=360)

