# -*- coding: utf-8 -*-
"""
Created on Sat Dec 23 23:03:10 2023

@author: Valentina Espinoza
"""

from to_myriam_eulervector import ROT_to_EulerVector, CSVsph_to_EulerVector, eulerVector_to_MYRIAM



def Matthews16_to_euvecTXT(plateID, output_path, reconstruction_times):
    
    data_dir = "assets/matthews_etal_2016" 
    
    if all(reconstruction_times) <= 250:
        rotation_path = "%s/Global_EB_250-0Ma_GK07_Matthews_etal.rot" %data_dir
        
    elif all(reconstruction_times) > 250 and all(reconstruction_times) <= 400:
        rotation_path = "%s/Global_EB_410-250Ma_GK07_Matthews_etal.rot" %data_dir
        
    else:
        raise ValueError("Input reconstruction_times beyond or accross the available time ranges (0 - 250 Ma and 250 - 400 Ma).")

    
    euler_vec_sph = ROT_to_EulerVector(rotation_path, plateID, reconstruction_times)
    
    eulerVector_to_MYRIAM(euler_vec_sph, output_path)
    
    
    
def GPlatesCSV_to_euvecTXT(plateID, output_path, delta_t):
    
    data_dir = "assets/matthews_etal_2016" 
    csv_path = "%s/equivalent_stage_rotation_comma_10.00Ma.csv" %data_dir
    
    
    euler_vec_sph = CSVsph_to_EulerVector(csv_path, plateID, delta_t)
    
    eulerVector_to_MYRIAM(euler_vec_sph, output_path)

    


if __name__ == "__main__":
    
    # FROM PYGPLATES
    plateID = 201 # South-America
    reconstruction_times = (10, 11)
    outputPath = r"eulerVector_ID%s_stg%s-%s_matthews_etal_2016_v1.txt" %(plateID, reconstruction_times[0], reconstruction_times[1])
    
    Matthews16_to_euvecTXT(plateID, outputPath, reconstruction_times)
    
    
    # FROM GPLATES CSV EXPORT
    plateID = 201 # South-America
    delta_t = 1
    outputPath = r"eulerVector_ID%s_matthews_etal_2016_v2.txt" %(plateID)
    
    GPlatesCSV_to_euvecTXT(plateID, outputPath, delta_t)
