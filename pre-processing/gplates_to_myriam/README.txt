19 September, 2023

MYRIAM: open-source software to estimate torque variations associated with plate-motion temporal changes
Espinoza, V., Martin de Blas, J., Iaffaldano, G.

This folder contains the following files:

gpml_to_myriam.py
examples/
  ├─ reconstructed_geometries.py
  ├─ resolved_topological_geometries.py
  ├─ assets/
	├─ matthews_etal_2016/
  	├─ seton_etal_2020/


The main file gpml_to_coordinatesTXT.py is a python script with fuctions to pre-process GPlates output files (GPML) to the input text-file format required by MYRIAM.
This script relies on the Python package 'pygplates' to (re) construct the boundary of an specific plate and provide with a .txt file containing the longitudes and latitudes of said boundary.
GPML files may contain features as 'Reconstructed Geometries' or 'Resolved Topological Geometries', each of which requires an specific handling in our script.

We provide with one example of each: 
- The script 'resolved_topological_geometries.py' uses GPMLs in 'assests/matthews_etal_2016/'. The example creates a coordinates file for the Nazca plate reconstructed to 2.0 Ma.
- The script 'reconstructed_geometries.py' uses GPML in 'assests/seton_etal_2020/'. The example creates a coordinates file for the South American Terrane reconstructed to 10.0 Ma.


Feel free to adapt the scripts to your specific needs.
If you run into any problem or have further questions, please do not hesitate to contact the authors via email (Valentina Espinoza, vf@ign.ku.dk).