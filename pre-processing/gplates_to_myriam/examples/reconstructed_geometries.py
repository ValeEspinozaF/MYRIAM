# -*- coding: utf-8 -*-
"""
Created on Tue Sep 19 17:35:31 2023

@author: nbt571
"""

# Add gplates_to_myriam path to PYTHONPATH!
from gpml_to_myriam import geometries_to_polygonFeature, polygonFeature_toMYRIAM



def Seton2020_to_terraneTXT(plateID, outputPath, reconstruction_time = 0.0):
    
    data_dir = "assets/seton_etal_2020" 
    
    if reconstruction_time <= 410:
        gpml_path = "%s/Global_EarthByte_GeeK07_COB_Terranes_2019_v3.gpml" %data_dir
        rot_path = "%s/Global_410-0Ma_Rotations_2019_v3_GeeK07.rot" %data_dir
        
    else:
        raise("Input reconstruction_time beyond the available time range (0 - 410 Ma).")
    
    geometryFeature =  geometries_to_polygonFeature(gpml_path, 
                                                    rot_path, 
                                                    plateID, 
                                                    reconstruction_time,
                                                    )
    
    polygonFeature_toMYRIAM(geometryFeature, outputPath)



if __name__ == "__main__":
    
    plateID = 952  # South American terrane
    reconstruction_time = 10.0
    outputPath = r"terrane_ID%s_age%s_seton_etal_2020.txt" % (plateID, reconstruction_time)
    
    Seton2020_to_terraneTXT(plateID, outputPath, reconstruction_time) 