# -*- coding: utf-8 -*-
"""
Created on Tue Sep 19 12:01:55 2023

@author: nbt571
"""

# Public dependencies
import os
import pygplates
import numpy as np


def topologies_to_polygonFeature(topologies_path, rotation_path, 
                                 plateID, reconstruction_time = 0.0):

    resolved_geometries = []
    pygplates.resolve_topologies(topologies_path, rotation_path, 
                                 resolved_geometries, reconstruction_time)
    
    filtered_geometries = []
    for topo in resolved_geometries:
        if topo.get_feature().get_reconstruction_plate_id() == plateID:
            filtered_geometries.append(topo.get_resolved_geometry())


    if len(filtered_geometries) == 0:
        raise("No feature found with the given plateID.")
    
    elif len(filtered_geometries) > 1:
        raise("More than one feature found with the given plateID.")
     
    return filtered_geometries[0]



def geometries_to_polygonFeature(geometries_path, rotation_path, 
                                 plateID, reconstruction_time = 0.0):

    reconstructed_geometries = []
    filtered_geometries = []
    
    output_parameters = dict(reconstruct_type = pygplates.ReconstructType.feature_geometry)
    
    pygplates.reconstruct(geometries_path, rotation_path,
                          reconstructed_geometries, reconstruction_time,
                          **output_parameters,
                          )
    
    
    for geom in reconstructed_geometries:
        ft = geom.get_feature()
        
        if ft.get_reconstruction_plate_id() == plateID: # Check ID
            
            if isinstance(geom.get_reconstructed_geometry(), pygplates.PolygonOnSphere): # Check if polygon
                
                filtered_geometries.append(geom.get_reconstructed_geometry())
            

    if len(filtered_geometries) == 0:
        raise("No feature found with the given plateID.")
    
    elif len(filtered_geometries) > 1:
        raise("More than one feature found with the given plateID.")
     
    return filtered_geometries[0]



def polygonFeature_toMYRIAM(polygon_ft, outputPath):
    
    if not isinstance(polygon_ft, pygplates.PolygonOnSphere):
        raise("Input feature must be an instance of pygplates.PolygonOnSphere.")
        
    
    geom_array = polygon_ft.to_lat_lon_array()
    geom_array_lon_lat = np.flip(geom_array, axis=1)
    
    with open(outputPath, 'w+') as datafile_id: 
         np.savetxt(datafile_id, 
                    geom_array_lon_lat,
                    comments='!',
                    fmt = ['%1.4e','%1.4e'],
                    header="lon(degE) lat(degN)",
                    )  