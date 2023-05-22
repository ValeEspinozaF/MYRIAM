# -*- coding: utf-8 -*-
"""
Created on Mon Oct 17 17:33:47 2022

@author: nbt571
"""

# Public dependencies
import os
import sys
import numpy as np
import pandas as pd
import cartopy.crs as ccrs
import matplotlib.pyplot as plt
import matplotlib.patches as mpatches
from math import atan2, sqrt


from MapFeatures import globalFeatures, gridLabels_inside, plot_poles
    

def cart2sph(xyz = []):
    """ 
    Transforms cartesian to spherical coordinates
    """

    x = xyz.iloc[:,0]
    y = xyz.iloc[:,1]
    z = xyz.iloc[:,2]


    # Set lat, lon empty lists
    lat = []
    lon = []    
           
        
    # Iterate and extend lists
    lat.extend( [atan2(Az, sqrt(Ax**2 + Ay**2)) for Ax, Ay, Az in zip(x,y,z)] )
    lon.extend( [atan2(Ay, Ax) for Ax, Ay in zip(x,y)] )

    
    # Turns lat, lon output from radians to degrees        
    lat = list(np.degrees(lat))
    lon = list(np.degrees(lon))
        
    return lat, lon


# Cmd inputs
plateAccronym = sys.argv[1]
TMP_Dir = sys.argv[2]
MTX_w2M_Dir = sys.argv[3]
dM_PDD_Dir = sys.argv[4]
runLabel = sys.argv[5]



# Load files
cntrNML_fileName = "CNTR_68.txt" 
cntrROT_fileName = "CNTR_ROT_68.txt" 
ensNML_fileName = "ENSdM.txt" 
ensROT_fileName = "ENSdM_ROT.txt" 

cntrNML_path = os.path.join(TMP_Dir, cntrNML_fileName)
cntrROT_path = os.path.join(TMP_Dir, cntrROT_fileName)
ensNML_path = os.path.join(TMP_Dir, ensNML_fileName)
ensROT_path = os.path.join(TMP_Dir, ensROT_fileName)

cntrNML = pd.read_csv(cntrNML_path, delimiter=' ', header=None, names=["lon", "lat"])
cntrROT = pd.read_csv(cntrROT_path, delimiter=' ', header=None, names=["lon", "lat"])
ensNML = pd.read_csv(ensNML_path, delimiter=' ', header=None, names=["x", "y", "z"])
ensROT = pd.read_csv(ensROT_path, delimiter=' ', header=None, names=["x", "y", "z"])



# Plate contour path
contourName = "BDR_%s.txt" %plateAccronym
contourPath = os.path.join(MTX_w2M_Dir, contourName)


# Set Figure
fig = plt.figure(constrained_layout=True, figsize=(9,9), dpi=360 )    
ax = fig.add_subplot(1, 1, 1, projection=ccrs.PlateCarree())


# Plot plate and continent contours
globalFeatures(ax, contourPath, plotGridLines=False)


# Plot grid lines
xLinspace = np.linspace(-180, 180, 19)
yLinspace = np.linspace(-90, 90, 13)
gridLabels_inside(ax, xLinspace, yLinspace)


# Figure title
plt.title('Torque-variation pole: Original (red) vs Rotated (blue)', fontsize=13, pad=10)


for ensemble, colour in zip([ensNML, ensROT], ['r', 'b']):
    
    # Plot poles
    if len(ensemble) > 10000:
        ensLat, ensLon = cart2sph(xyz = ensemble[['x','y','z']][:10000])
    
    else:
        ensLat, ensLon = cart2sph(xyz = ensemble[['x','y','z']])
        
        
    plot_poles(ax, ensLat, ensLon, color=colour)
    
    
    
for contour, colour in zip([cntrNML, cntrROT], ['darkred', 'darkblue']):
    
    # Plot contour
    
    # Split Dataframes where nan rows appear (when multiple contours are stacked)
    contour["cntrNr"] = contour.isnull().all(axis=1).cumsum()
    
    for n, rows in contour.groupby("cntrNr").groups.items():
        
        indCntr = contour.iloc[rows].drop(columns="cntrNr", axis=1).dropna()
    
        poly = mpatches.Polygon(indCntr, closed=True, ec=colour, fill=False, 
                                lw=0.8, fc=None, transform=ccrs.PlateCarree(), zorder=9)
        ax.add_patch(poly)
        
        
        
# Save figure as png
figName = "MAP_ROTATED_CNTR68_%s.png" %runLabel
figpath = os.path.join(dM_PDD_Dir, figName)       
plt.savefig(figpath, bbox_inches='tight', dpi=360)
