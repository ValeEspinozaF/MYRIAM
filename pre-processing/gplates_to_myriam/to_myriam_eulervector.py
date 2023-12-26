# -*- coding: utf-8 -*-
"""
Created on Sat Dec 23 02:41:06 2023

@author: Valentina Espinoza
"""

# Public dependencies
import numpy as np
import pandas as pd
import pygplates


# Contains
# 1. ROT_to_EulerVector
# 2. CSVsph_to_EulerVector
# 3. eulerVector_to_MYRIAM


def ROT_to_EulerVector(rotation_path, plateID, reconstruction_times):
    """
    Calculates an Euler Vector of plate rotation for a given .ROT file, a
    plateID and the reconstructions times for the stage.

    Parameters
    ----------
    rotation_path : string
        Path to a .ROT file. The file is expected to have the format of a 
        GPlates "Reconstruction Tree" datatype.
    plateID : int
        ID of the plate, for the given .ROT file.
    reconstruction_times : tuple
        Times of stage reconstruction. Must contains exactly two numbers.

    Returns
    -------
    array
        Euler vector expressed as longitude (deg), latitude (deg) and angular 
        velocity (deg/Myr).

    """
    
    if len(reconstruction_times) != 2:
        raise ValueError("Input reconstruction_times must be contain exactly two elements.")
        
    if not all(isinstance(x, int) for x in reconstruction_times) and not all(isinstance(x, float) for x in reconstruction_times):
        raise TypeError("Input reconstruction_times elements must be numbers.")
        
    if reconstruction_times[0] >= reconstruction_times[1]:
        raise ValueError("Input reconstruction_times elements must be increasing in value.")

    rotation_model = pygplates.RotationModel(rotation_path)
    equivalent_stage_rotation = rotation_model.get_rotation(
        reconstruction_times[0], plateID, reconstruction_times[1])

    stg_finrot = equivalent_stage_rotation.get_euler_pole_and_angle()
    delta_t = reconstruction_times[1] - reconstruction_times[0]

    euler_vec_sph = np.array([[stg_finrot[0].to_lat_lon()[1]],
                              [stg_finrot[0].to_lat_lon()[0]],
                              [np.degrees(stg_finrot[1]) / delta_t],
                              ])
    
    return euler_vec_sph



def CSVsph_to_EulerVector(csv_path, plateID, delta_t):
    """
    Calculates an Euler Vector of plate rotation for a given .CSV file, a
    plateID and the stage time lenght (delta_t).
    

    Parameters
    ----------
    csv_path : TYPE
        Path to a .CSV file. The file is expected to have the format of a 
        GPlates exported "Equivalent Stage Rotation" datatype in spherical 
        coordinates.         
    plateID : int
        ID of the plate, for the given .CSV file.
    delta_t : TYPE
        Time interval of the stage rotation .

    Returns
    -------
    array
        Euler vector expressed as longitude (deg), latitude (deg) and angular 
        velocity (deg/Myr).

    """
    
    data_fr = pd.read_csv(csv_path, skiprows=[0], names=["plateID", "pole_lat", "pole_lon", "angle"])
    stg_finrot = data_fr[data_fr["plateID"] == plateID]
    euler_vec_sph = np.array([[float(stg_finrot["pole_lon"])],
                              [float(stg_finrot["pole_lat"])],
                              [float(stg_finrot["angle"]) / delta_t],
                              ])
    
    return euler_vec_sph
        
 

def eulerVector_to_MYRIAM(euler_vec_sph, output_path):
    """
    Saves a .TXT file in MYRIAM input format. The output is a three-column array 
    of a sigle Euler vector expressed spherical coordinates.
    

    Parameters
    ----------
    euler_vec_sph : array
        Euler vector expressed as longitude (deg), latitude (deg) and angular 
        velocity (deg/Myr).
    output_path : string
        Path to a .TXT file.

    Returns
    -------
    None.

    """
    
    with open(output_path, 'w+') as datafile_id: 
         np.savetxt(datafile_id, 
                    euler_vec_sph.T,
                    comments='!',
                    fmt = ['%1.4e','%1.4e','%1.4e'],
                    header="lon(degE) lat(degN) angle(deg/Myr)",
                    )  