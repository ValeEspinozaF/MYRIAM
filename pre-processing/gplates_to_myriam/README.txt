26 December, 2023

MYRIAM: open-source software to estimate torque variations associated with plate-motion temporal changes
Espinoza, V., Martin de Blas, J., Iaffaldano, G.

This folder contains the following files:

to_myriam_contours.py
to_myriam_eulervector.py
examples/
  ├─ reconstructed_geometries.py
  ├─ resolved_topological_geometries.py
  ├─ equivalent_stage_rotation.py
  └─ assets/
	├─ matthews_etal_2016/
  	└─ seton_etal_2020/


Both files to_myriam_contours.py and to_myriam_eulervector.py are a python scripts with fuctions to pre-process GPlates files (GPML/ROT) to the input text-file format required by MYRIAM. These script rely on the Python package 'pygplates' to read, filter and reconstruct the boundary/motion of an specific plate/feature.


## BOUNDARY RECONSTRUCTION ##
GPML files may contain features as 'Reconstructed Geometries' or 'Resolved Topological Geometries', each of which requires an specific handling. We provide with one example of each, via two possible workflows: 

- The script 'resolved_topological_geometries.py' creates a coordinates TXT file for the Nazca plate reconstructed to 10.0 Ma, based on the plate boundary evolution from Matthews et al. 2016 (assets in 'assests/matthews_etal_2016/'). This is achieved by a) using a GPML and ROT file to reconstruct Nazca boundaries using pygplates, or b) using GPlates to export the "Resolved Topological Geometries" polygons as shapefile.

- The script 'reconstructed_geometries.py' creates a coordinates TXT file for the South American Terrane (SAT) reconstructed to 10.0 Ma, based on the plate boundary evolution from Seton et al. 2020 (assets in 'assests/seton_etal_2020/'). This is achieved by a) using a GPML and ROT file to reconstruct SAT boundaries using pygplates, or b) using GPlates to export the "Reconstructed Geometries" polygons as shapefile.


## MOTION RECONSTRUCTION ##
ROT files hold relative finite rotations. One can use pygplates or GPlates to calculate a "Equivalent Stage Rotation". The script 'equivalent_stage_rotation.py' creates an Euler vector TXT file for the motion of the South-America plate between 10.0 and 11.0 Ma, based on the plate reconstruction from Matthews et al. 2016 (assets in 'assests/matthews_etal_2016/'). This is achieved by a) using a ROT file to get stage motion using pygplates, or b) using GPlates to export the "Equivalent Stage Rotation" motions as CSV (comma delimited).



Feel free to adapt the scripts to your specific needs.
If you run into any problem or have further questions, please do not hesitate to contact the authors via email (Valentina Espinoza, vf@ign.ku.dk).