# -*- coding: utf-8 -*-
"""
Created on Tue Sep 19 17:17:12 2023

@author: nbt571
"""

from to_myriam_contours import GPML_topologies_to_coordinates, SPH_to_coordinates, coordinates_to_MYRIAM



def Matthews16_to_plateTXT(plateID, outputPath, reconstruction_time = 0.0):
    
    data_dir = "assets/matthews_etal_2016" 
    
    if reconstruction_time <= 250:
        gpml_path = "%s/Global_EarthByte_Mesozoic-Cenozoic_plate_boundaries_Matthews_etal.gpml" %data_dir
        rotation_path = "%s/Global_EB_250-0Ma_GK07_Matthews_etal.rot" %data_dir
        
    elif reconstruction_time > 250 and reconstruction_time <= 400:
        gpml_path = "%s/Global_EarthByte_Paleozoic_plate_boundaries_Matthews_etal.gpml" %data_dir
        rotation_path = "%s/Global_EB_410-250Ma_GK07_Matthews_etal.rot" %data_dir
        
    else:
        raise ValueError("Input reconstruction_time beyond the available time range (0 - 400 Ma).")

    
    coords_lonlat =  GPML_topologies_to_coordinates(gpml_path, rotation_path, 
                                                    plateID, reconstruction_time)
    
    coordinates_to_MYRIAM(coords_lonlat, outputPath)



def SPH_to_plateTXT(plateID, outputPath):
    
    data_dir = "assets/matthews_etal_2016" 
    shapefile_path = "%s/topology_10.00Ma.shp" %data_dir
    
    coords_lonlat =  SPH_to_coordinates(shapefile_path, plateID)
    coordinates_to_MYRIAM(coords_lonlat, outputPath)



if __name__ == "__main__":
    
    
    # FROM GPML
    plateID = 911  # Nazca plate
    reconstruction_time = 10.0
    outputPath = r"plateBoundary_ID%s_matthews_etal_2016.txt" %plateID
    Matthews16_to_plateTXT(plateID, outputPath, reconstruction_time)
    
    
    # FROM SHAPEFILE
    plateID = 911  # Nazca plate
    outputPath = r"plateBoundary_ID%s_matthews_etal_2016_v2.txt" %plateID
    SPH_to_plateTXT(plateID, outputPath)