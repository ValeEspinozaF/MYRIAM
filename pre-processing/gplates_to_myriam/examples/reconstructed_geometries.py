# -*- coding: utf-8 -*-
"""
Created on Tue Sep 19 17:35:31 2023

@author: nbt571
"""

from to_myriam_contours import GPML_geometries_to_coordinates, SPH_to_coordinates, coordinates_to_MYRIAM



def Seton2020_to_terraneTXT(plateID, outputPath, reconstruction_time = 0.0):
    
    data_dir = "assets/seton_etal_2020" 
    
    if reconstruction_time <= 410:
        gpml_path = "%s/Global_EarthByte_GeeK07_COB_Terranes_2019_v3.gpml" %data_dir
        rotation_path = "%s/Global_410-0Ma_Rotations_2019_v3_GeeK07.rot" %data_dir
        
    else:
        raise ValueError("Input reconstruction_time beyond the available time range (0 - 410 Ma).")
    
    coords_lonlat =  GPML_geometries_to_coordinates(gpml_path, rotation_path, 
                                                    plateID, reconstruction_time)
    
    coordinates_to_MYRIAM(coords_lonlat, outputPath)



def SPH_to_plateTXT(plateID, outputPath):
    
    data_dir = "assets/seton_etal_2020" 
    shapefile_path = "%s/reconstructed_10.00Ma/reconstructed_10.00Ma_polygon.shp" %data_dir
    
    coords_lonlat =  SPH_to_coordinates(shapefile_path, plateID)
    coordinates_to_MYRIAM(coords_lonlat, outputPath)
    
    

if __name__ == "__main__":
    
    
    # FROM GPML
    plateID = 952  # South American terrane
    reconstruction_time = 10.0
    outputPath = r"terrane_ID%s_age%s_seton_etal_2020.txt" % (plateID, reconstruction_time)
    Seton2020_to_terraneTXT(plateID, outputPath, reconstruction_time) 
    
    
    # FROM SHAPEFILE
    plateID = 952  # South American terrane
    outputPath = r"terrane_ID%s_seton_etal_2020.txt" %plateID
    SPH_to_plateTXT(plateID, outputPath)