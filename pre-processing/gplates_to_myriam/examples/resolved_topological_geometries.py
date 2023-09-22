# -*- coding: utf-8 -*-
"""
Created on Tue Sep 19 17:17:12 2023

@author: nbt571
"""

from gpml_to_plateBoundaryTXT import topologies_to_polygonFeature, polygonFeature_toMYRIAM



def Matthews16_to_plateTXT(plateID, outputPath, reconstruction_time = 0.0):
    
    data_dir = "assets/matthews_etal_2016" 
    
    if reconstruction_time <= 250:
        gpml_path = "%s/Global_EarthByte_Mesozoic-Cenozoic_plate_boundaries_Matthews_etal.gpml" %data_dir
        rot_path = "%s/Global_EB_250-0Ma_GK07_Matthews_etal.rot" %data_dir
        
    elif reconstruction_time > 250 and reconstruction_time <= 400:
        gpml_path = "%s/Global_EarthByte_Paleozoic_plate_boundaries_Matthews_etal.gpml" %data_dir
        rot_path = "%s/Global_EB_410-250Ma_GK07_Matthews_etal.rot" %data_dir
        
    else:
        raise("Input reconstruction_time beyond the available time range (0 - 400 Ma).")

    
    geometryFeature =  topologies_to_polygonFeature(gpml_path, rot_path, 
                                                    plateID, reconstruction_time)
    
    polygonFeature_toMYRIAM(geometryFeature, outputPath)



if __name__ == "__main__":
    
    plateID = 911  # Nazca plate
    reconstruction_time = 2.0
    outputPath = r"plateBoundary_ID%s_matthews_etal_2016.txt" %plateID
    
    Matthews16_to_plateTXT(plateID, outputPath, reconstruction_time)