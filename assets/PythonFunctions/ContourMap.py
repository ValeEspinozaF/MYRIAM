# -*- coding: utf-8 -*-
"""
Created on Tue Sep 20 11:25:49 2022

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


from MapFeatures import globalFeatures, gridLabels_inside


# Cmd inputs
contourLabel = sys.argv[1]
plateAccronym = sys.argv[2]
MTX_w2M_Dir = sys.argv[3]
dM_PDD_Dir = sys.argv[4]


# Load confidence contours
cntr20_fileName = "CNTR20%s.txt" %contourLabel
cntr68_fileName = "CNTR68%s.txt" %contourLabel

cntr20_path = os.path.join(dM_PDD_Dir, cntr20_fileName)
cntr68_path = os.path.join(dM_PDD_Dir, cntr68_fileName)

try:
    cntr20 = pd.read_csv(cntr20_path, delimiter=' ', header=None)
except pd.errors.EmptyDataError:
    cntr20 = pd.DataFrame()

try:
    cntr68 = pd.read_csv(cntr68_path, delimiter=' ', header=None)
except pd.errors.EmptyDataError:
    cntr68 = pd.DataFrame()


# Plate contour path
contourName = "BDR_%s.txt" %plateAccronym
contourPath = os.path.join(MTX_w2M_Dir, contourName)


# Set Figure
fig = plt.figure(constrained_layout=True, figsize=(9,9), dpi=360 )    
ax = fig.add_subplot(1, 1, 1, projection=ccrs.PlateCarree())


# Plot plate and continent contours
globalFeatures(ax, contourPath, plotGridLines=False)


# Plot grid lines
xLinspace = np.arange(-180, 181, 20)[1:-1]
yLinspace = np.arange(-90, 91, 15)[1:-1]
gridLabels_inside(ax, xLinspace, yLinspace)


# Figure title
plt.title('Torque-variation pole distribution (20% and 68%)', fontsize=13, pad=10)


# Plot Ellipse Contour    
for contour, lwidth, lstyle in zip([cntr20, cntr68], [1.2, 0.8], [":", "-"]):
    
    if not contour.empty:

        # Split Dataframes where nan rows appear (when multiple contours are stacked)
        contour["cntrNr"] = contour.isnull().all(axis=1).cumsum()

        for n, rows in contour.groupby("cntrNr").groups.items():

            indCntr = contour.iloc[rows].drop(columns="cntrNr", axis=1).dropna()

            poly = mpatches.Polygon(indCntr, closed=True, ec='r', fill=False, ls=lstyle,
                                    lw=lwidth, fc=None, transform=ccrs.PlateCarree(), zorder=9)
            ax.add_patch(poly)


# Save figure as png
figName = "MAP_CNTR20c68%s.png" %contourLabel
figpath = os.path.join(dM_PDD_Dir, figName)       
plt.savefig(figpath, bbox_inches='tight', dpi=360)