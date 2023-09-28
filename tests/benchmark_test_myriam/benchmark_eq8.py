# -*- coding: utf-8 -*-
"""
Created on Wed Sep 27 08:42:58 2023

@author: nbt571
"""

from numpy import pi
from math import radians, cos


# Earth's radius in m:
Re = 6371e3

#Viscosities of asthenosphere and upper mantle, in Pa*s:
muA = 5e19
muM = 1.5e21

# Gamma (in m) and thickness of asthenosphere and lithosphere (in m).
gamma = 4.73e5
Ha = gamma * (muA / muM)**(1/3) # From Paulson & Richards (2009)
Hl = 180e3

# Euler vector change -- Angular velocity in deg/Myr and rad/s.
AV_deg_Myr = 1
AV_rad_s = radians(AV_deg_Myr) / (1e6*365*24*60*60)

# Radius to the bottom of the lithosphere in m.
r = Re - Hl


# Half-length values:
# plate_halflength: We try three different values: 2.8, 16.6 and 35.2
plate_halflength = 2.8


lat_rad = radians(plate_halflength)
colat_rad = pi/2 - lat_rad #colalitude

# Calculate torque-change magnitude (DeltaM):
DeltaM = (2*(AV_rad_s) * muA * r**4 * (pi-2*colat_rad)/Ha) * (1/3*(cos(pi-colat_rad))**3 - cos(pi-colat_rad))
print(DeltaM)