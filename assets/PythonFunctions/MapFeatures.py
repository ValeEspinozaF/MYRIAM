# -*- coding: utf-8 -*-
"""
Created on Tue Sep 20 11:49:16 2022

@author: nbt571
"""

import os
import numpy as np
import pandas as pd
from pathlib import Path
import matplotlib.pyplot as plt
import cartopy.crs as ccrs
from cartopy.io.shapereader import Reader
from cartopy.feature import ShapelyFeature
from cartopy.mpl.ticker import (LongitudeFormatter, LatitudeFormatter)


# Main project workspace
projFolder = Path(__file__).parents[2]



def setCartographic_AxisLabels(ax):
    
    # Ticks range
    ymin0, ymax0 = ax.get_ylim()
    xmin0, xmax0 = ax.get_xlim()
        
    
    # Ensure min starts at factor of 5 (if extent is not too small)
    if ymax0 - ymin0 > 5: 
        ymin, ymax = np.round( (ymin0-5)/5 ) * 5, ymax0
    else:
        ymin, ymax = np.floor(ymin0), np.ceil(ymax0)
        
        
    if xmax0 - xmin0 > 5: 
        xmin, xmax = np.round( (xmin0-5)/5 ) * 5, xmax0
    else:
        xmin, xmax = np.floor(xmin0), np.ceil(xmax0)
        
    
    # Set ticks
    yTickList = []
    xTickList = []
    step_sizes = [10, 5, 2, 1, 0.5]

    for step in step_sizes:
        yTickList = np.arange(ymin, ymax, step)
        if len(yTickList) >= 5:
            break
        
    for step in step_sizes:
        xTickList = np.arange(xmin, xmax, step)
        if len(xTickList) >= 5:
            break
    
    
    # Set labels
    xTickLabels = [""] * len(xTickList)
    for i in range(len(xTickList)):
        xtick = xTickList[i]
        
        if step >= 1:
            xTickLabels[i] = "%d$^\circ$" %xtick
        else:
            xTickLabels[i] = "%.1f$^\circ$" %xtick
        
        if xtick < 0:
            xTickLabels[i] += "W"
        elif xtick > 0:
            xTickLabels[i] += "E"
            
            
    yTickLabels = [""] * len(yTickList)
    for i in range(len(yTickList)):
        ytick = yTickList[i]
        
        if step >= 1:
            yTickLabels[i] = "%d$^\circ$" %ytick
        else:
            yTickLabels[i] = "%.1f$^\circ$" %ytick
        
        if ytick < 0:
            yTickLabels[i] += "S"
        elif ytick > 0:
            yTickLabels[i] += "N" 
    
    
    ax.set(
        yticks = yTickList,
        xticks = xTickList,
        yticklabels = yTickLabels,
        xticklabels = xTickLabels,
        ylim = (ymin0, ymax0),
        xlim = (xmin0, xmax0),
    )





# Workaround low threshold on geodetic lines across the globe
class LwThOrthographic(ccrs.Orthographic):

    @property
    def threshold(self):
        return 1e3
    
    
    
def globalFeatures(ax, contourPath='', plotGridLines=True):
    
    # Plot global rather than zoom into the extents of any plotted data
    ax.set_global() 
    
    
    # Coastlines
    ax.add_feature(coastlines_feature(edgecolor='none', facecolor='#E6E6E6', lw=0))
    
    
    # Plate boundaries
    ax.add_feature(plateBoundaries_feature())
    
    
    # Specific boundary for given plate
    if contourPath != '':
        plateContour_feature(ax, contourPath, edgecolor = '0.7', lw=1.7)

    
    # Grid lines    
    if plotGridLines:
        ax.gridlines(linewidth=0.3, color='black', alpha=0.8, linestyle='--')
        
        

def plateBoundaries_feature(edgecolor = '0.25', facecolor='none', lw=0.6):
    
    # Input shapefile
    folderPath = 'assets\Shapefiles'
    inputShp_plates = 'PlateBoundaries_PolyLine.shp' 
    inputShp_plates = os.path.join(projFolder, folderPath, inputShp_plates)


    # Create Feature
    shape_feature = ShapelyFeature(Reader(inputShp_plates).geometries(),
                                   ccrs.PlateCarree(),
                                   edgecolor = edgecolor, facecolor=facecolor, lw=lw)
    return shape_feature



def coastlines_feature(edgecolor = '#AAAAAA', facecolor='none', lw=0.4):
    
    # Input shapefile
    folderPath = 'assets\Shapefiles'
    inputShp_coastlines = 'Coastlines_Polygon.shp' 
    inputShp_coastlines = os.path.join(projFolder, folderPath, inputShp_coastlines)
    
    
    # Create Feature
    shape_feature = ShapelyFeature(Reader(inputShp_coastlines).geometries(),
                                    ccrs.PlateCarree(),
                                    edgecolor = edgecolor, facecolor=facecolor, lw=lw)
    return shape_feature
        
        
        
def plateContour_feature(ax, contourPath,
                         edgecolor = '0.7', lw=0.7):
    
    # Load plate contour
    contourXY = pd.read_csv(contourPath, delimiter=' ', names=["lon", "lat"])
    ax.plot(contourXY["lon"], contourXY["lat"], '-', linewidth=lw, color=edgecolor )
    


def gridLabels_inside(ax, xLinspace, yLinspace):
    
    ax.set_xticks(xLinspace, crs=ccrs.PlateCarree())
    ax.set_yticks(yLinspace, crs=ccrs.PlateCarree())
    
    # Format to display degrees (NS, EW)
    lon_formatter = LongitudeFormatter(zero_direction_label=True)
    lat_formatter = LatitudeFormatter()
    ax.xaxis.set_major_formatter(lon_formatter)
    ax.yaxis.set_major_formatter(lat_formatter)
    
    # Get label on the inside
    ax.tick_params(axis="y",direction="in", length=2, pad=-5)
    ax.tick_params(axis="x",direction="in", length=2, pad=-11)
    
    # Put ticks at the top and right axes
    ax.yaxis.tick_right()
    ax.xaxis.tick_top()
    
    # Format the label
    labelParams = {
        "fontsize" : 7, 
        "color" : "dimgray",
        #"backgroundcolor" : "w"
        }

    plt.setp(ax.get_xticklabels(), **labelParams)
    plt.setp(ax.get_yticklabels(), ha="right", **labelParams)
    
    # Plot grid
    gridParams = {
        "linewidth" : 0.2, 
        "color" : '0.3', 
        "alpha" : 0.5, 
        "linestyle" : '--',
        }

    ax.gridlines(crs=ccrs.PlateCarree(), xlocs=xLinspace, ylocs=yLinspace,
                 **gridParams)
    
    

def plot_poles(ax, lat, lon, 
               color='black', markersize=2):
    
    # Plot pole
    ax.plot(lon, lat, 
            transform=ccrs.PlateCarree(),
            marker='+', color=color, mew = 0.4, 
            markersize=markersize, linewidth=0) 
    