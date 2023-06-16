# -*- coding: utf-8 -*-
"""
Created on Tue Oct 18 10:16:31 2022

@author: nbt571
"""

# Public dependencies
import os
import sys
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from matplotlib.ticker import ScalarFormatter


def set_ytickLabels(ax):

    # Set tick class    
    class ScalarFormatterClass(ScalarFormatter):
       def _set_format(self):
          self.format = "%.1f"
    
    # Apply format
    axScalarFormatter = ScalarFormatterClass(useMathText=True)
    axScalarFormatter.set_powerlimits((0,0))
    ax.yaxis.set_major_formatter(axScalarFormatter)
    ax.xaxis.set_major_formatter(axScalarFormatter)




# Cmd inputs
modelLabel = sys.argv[1]
repositoryDir = sys.argv[2]


# Load file
magHist_fileName = "MAGHIST_%s.txt" %modelLabel
magHist_path = os.path.join(repositoryDir, magHist_fileName)
magHist = pd.read_csv(magHist_path, delimiter=' ', header=None, names=["x", "y"])

nSize = sum(magHist["y"])


# Set main figure
fig = plt.figure(figsize=(5,5), dpi=480)    
ax = fig.add_subplot(111)
set_ytickLabels(ax)
ax.grid(True, linewidth=0.3, alpha=0.5)

ax.set_xlabel('Magnitude [N*m]', labelpad=10)
ax.set_ylabel('Frequency (size = %.0e)' %nSize, labelpad=10)
ax.set_title("Torque-variation Magnitude", pad=10)


# Plot Histogram
plt.hist(magHist["x"], weights=magHist["y"], bins = len(magHist), ec="1")


# Save figure as png
figName = "PLOT_MAGHIST_%s.png" %modelLabel
figpath = os.path.join(repositoryDir, figName)       
plt.savefig(figpath, bbox_inches='tight', dpi=360)