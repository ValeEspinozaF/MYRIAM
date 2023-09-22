Global plate boundary evolution and kinematics since the late Paleozoic 

Kara J. Matthews*^, Kayla T. Maloney*, Sabin Zahirovic*, Simon E. Williams*, Maria Seton*, R. Dietmar Müller*

* EarthByte Group, School of Geosciences, The University of Sydney, Sydney, NSW 2006, Australia
^ Present address: Department of Earth Sciences, University of Oxford, South Parks Road, Oxford OX1 3AN, UK

Contact: karajmatthews@gmail.com

CORRECTION applied for the Pacific plate prior to 83 Ma based on Torsvik et al. (2019)


Supplementary Material

We provide a digital plate model files (including rotations and geometries) with this publication. These files allow for the visualisation and/or manipulation of the late Paleozoic to present-day (410-0 Ma) global plate motion model presented in this study. 

#########################################
The digital plate model files are compatible with the open-source GPlates plate reconstruction software (www.gplates.org):

(1) Rotations - Global rotation model that contains the reconstruction poles that describe the motions of the continents and oceans.
- Global_EB_250-0Ma_GK07_Matthews_etal.rot (455 KB)
- Global_EB_410-250Ma_GK07_Matthews_etal.rot (115 KB) - in the comments 'POLE_RECALCULATED' means that we recalculated that finite pole of rotation such that the moving plate moves relative to a neighbouring plate rather than directly to the absolute reference frame (see Section 2.2.1 of the main text for more details). This process should have a minimal effect on the absolute motion of the plate.

(2) Plate polygons and boundary geometries - Topologically closed plate polygons are constructed from the intersection of ridges, transforms, subduction zones and other plate boundary geometries. These 'resolved topologies' are valid at 1 Myr intervals (410-0 Ma). The plate boundary geometries and plate polygons have been assigned plate reconstruction IDs to allow them to be reconstructed using the supplied rotation file.
- Global_EarthByte_Mesozoic-Cenozoic_plate_boundaries_Matthews_etal.gpml (36 MB)
- Global_EarthByte_Paleozoic_plate_boundaries_Matthews_etal.gpml (8.7 MB)
- TopologyBuildingBlocks_Matthews_etal.gpml (2 MB) - this file has not been modified from Müller et al. (2016)

(3) Coastlines - Geometries of the present-day coastlines.
- Global_coastlines_low_res_Matthews_etal.gpml (25.4 MB)
- Global_coastlines_low_res_Matthews_etal.shp (2.9 MB  inc. auxillary files, datum-WGS 1984)
NOTE: From 410 to 320-310 Ma Kazakhstania is represented as one or two ('Internal' and 'External' Kazakhstania - Domeier and Torsvik, 2014) ovate polygons. Kazakhstania is highly deformed following a long and complicated history, and so for simplicity we avoid using their present-day outlines in the earlier part of the model.

(4) Static polygons (optional) - Includes ocean isochron and terrane polygon geometries.
- Global_EarthByte_GPlates_PresentDay_StaticPlatePolygons_Matthews_etal.shp (2.7 MB inc. auxillary files, datum-WGS 1984)

(5) Continenal polygons (optional) - Includes continental terrane polygon geometries and excludes oceanic lithosphere.
- Global_EarthByte_GPlates_PresentDay_ContinentalPolygons_Matthews_etal.shp (804 KB inc. auxillary files, datum-WGS 1984)

GPLATES: 
To view the model load all files in GPlates (either drag and drop files onto the globe OR from the navigation bar at the top of the screen click File -> Open Feature Collection and select files). Both rotation files (1) and each of the three plate geometry files (2) need to be loaded for the model to work properly. It is recommended that coastlines (3) are loaded to see how the continents move, however only one coastline file is necessary (.gpml or .shp). The static polygons (4) and continental polygons (5) are optional. 

The two rotation files need to be 'connected' in order for the model to run continuously from 410 to 0 Ma. In the GPlates 'Layers' window (opened from the main navigation bar, click 'Window' -> 'Show Layers') the rotation files will be highlighted yellow, yet only one will have a yellow tick next to it to signify it is being used. Click the small black triangle to the left the ticked rotation file. Under 'Inputs' -> 'Reconstruction features' click 'Add new connection' and then select the other rotation file from the list of files that will appear. This will ensure that both rotation files are active. 

Finally, it is recommended to experiment with geometry visibility in order to make the globe less cluttered. For instance, from the navigation bar click View -> Geometry Visibility and untick 'Show Line Geometries'. Alternatively, files can be toggled on and off using the tick boxes in the Layers window. For more information about using GPlates, a set of user tutorials can be accessed from the GPlates website - http://www.gplates.org/docs.html.


#########################################
We also provide a list of the plate reconstruction IDs used in the model:

Plate IDs - A list of all the plate IDs used in the rotation and geometry files and their corresponding plate names.
- Global_EarthByte_Plate_ID_Table_Matthews_etal.txt (33 KB)

#########################################
MODEL REFERENCING:
When using our model, in addition to citing this publication:

Matthews, K.J., Maloney, K.T., Zahirovic, S., Williams, S.E., Seton, M. and Müller, R.D., 2016, Global plate boundary evolution and kinematics since the late Paleozoic, Global and Planetary Change, in press, accepted 3 October 2016.

please also consider citing the studies of Domeier and Torsvik (2014) and Müller et al. (2016) which served as the basis for this model in the late Paleozoic and Mesozoic-Cenozoic, respectively, and cite any other study that describes refinements to the plate reconstructions in your region of interest. See Section 2 and Section 3 of the main text for more information on how the present model was constructed.

- Domeier, M., & Torsvik, T. H. (2014). Plate tectonics in the late Paleozoic. Geoscience Frontiers, 5(3), 303-350. DOI:10.1016/j.gsf.2014.01.002
- Müller, R. D., Seton, M., Zahirovic, S., Williams, S. E., Matthews, K. J., Wright, N. M., Shephard, G. E., Maloney, K., Barnett-Moore, N., Hosseinpour, M., Bower, D. J., & Cannon, J. (2016). Ocean Basin Evolution and Global-Scale Plate Reorganization Events Since Pangea Breakup. Annual Review of Earth and Planetary Sciences, 44(1). DOI:10.1146/annurev-earth-060115-012211

Note: We have recently fixed some issues in this model, namely the motion of the Pacific plate (following Torsvik et al., 2019), and some MOR topologies in the Arctic. The fixes are in the model files included in this folder, but the old (published) version of the model is included in a sub-folder called "_OLD_MODEL_DO_NOT_USE". 

Torsvik, T. H., B. Steinberger, G. E. Shephard, P. V. Doubrovine, C. Gaina, M. Domeier, C. P. Conrad, and W. W. Sager (2019), Pacific‐Panthalassic reconstructions: Overview, errata and the way forward, Geochemistry, Geophysics, Geosystems, 20(7), 3659-3689.


