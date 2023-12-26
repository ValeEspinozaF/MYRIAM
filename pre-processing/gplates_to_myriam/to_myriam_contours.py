# -*- coding: utf-8 -*-
"""
Created on Tue Sep 19 12:01:55 2023

@author: Valentina Espinoza
"""

# Public dependencies
import numpy as np
import pygplates


# Contains
# 1. GPML_topologies_to_coordinates
# 2. GPML_geometries_to_coordinates
# 3. SPH_to_coordinates
# 4. coordinates_to_MYRIAM


def GPML_topologies_to_coordinates(topologies_path, rotation_path, 
                                   plateID, reconstruction_time = 0.0):
    """
    Extracts the contour of a reconstructed feature from a given GPML file, 
    and outputs a two-column array of coordinates expressed in longitude (deg) 
    and latitude (deg). 

    Parameters
    ----------
    topologies_path : string
        Path to a GPML file. The file is expected to have the format of a 
        GPlates "Resolved Topological Geometries" datatype.
    rotation_path : string
        Path to a .ROT file. The file is expected to have the format of a 
        GPlates "Reconstruction Tree" datatype.
    plateID : int
        ID of the plate, for the given .ROT file.
    reconstruction_time : float, optional
        Times of reconstruction. The default is 0.0.

    Returns
    -------
    array
        Two-column array on longitudes and latitudes expressed in degrees.

    """

    resolved_geometries = []
    pygplates.resolve_topologies(topologies_path, rotation_path, 
                                 resolved_geometries, reconstruction_time)
    
    filtered_geometries = []
    for topo in resolved_geometries:
        if topo.get_feature().get_reconstruction_plate_id() == plateID:
            filtered_geometries.append(topo.get_resolved_geometry())


    if len(filtered_geometries) == 0:
        raise IndexError("No feature found with the given plateID.")
    
    elif len(filtered_geometries) > 1:
        raise IndexError("More than one feature found with the given plateID.")
     
    polygon_ft = filtered_geometries[0]
    geom_coords = polygon_ft.to_lat_lon_array()
    coords_lonlat = np.flip(geom_coords, axis=1)
    
    return coords_lonlat



def GPML_geometries_to_coordinates(geometries_path, rotation_path, 
                                   plateID, reconstruction_time = 0.0):
    """
    Extracts the contour of a reconstructed feature from a given GPML file, 
    and outputs a two-column array of coordinates expressed in longitude (deg) 
    and latitude (deg). 

    Parameters
    ----------
    geometries_path : string
        Path to a GPML file. The file is expected to have the format of a 
        GPlates "Reconstructed Geometries" datatype.         
    rotation_path : string
        Path to a .ROT file. The file is expected to have the format of a 
        GPlates "Reconstruction Tree" datatype.
    plateID : int
        ID of the plate, for the given .ROT file.
    reconstruction_time : float, optional
        Times of reconstruction. The default is 0.0.

    Returns
    -------
    array
        Two-column array on longitudes and latitudes expressed in degrees.

    """

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
        raise IndexError("No feature found with the given plateID.")
    
    elif len(filtered_geometries) > 1:
        raise IndexError("More than one feature found with the given plateID.")
     
    polygon_ft = filtered_geometries[0]
    geom_coords = polygon_ft.to_lat_lon_array()
    coords_lonlat = np.flip(geom_coords, axis=1)
    
    return coords_lonlat



def SPH_to_coordinates(shapefile_path, plateID):    
    """
    Extracts the contour of a polygon (or multipolygon) from a given shapefile, 
    and outputs a two-column array of coordinates expressed in longitude (deg) 
    and latitude (deg). If multipolygon, features are separated with nan rows.

    Parameters
    ----------
    shapefile_path : string
        Path to a .SPH polygon file. The file is expected to have the format 
        of a GPlates exported "Reconstructed Geometries" or "Resolved 
        Topological Geometries" datatype.         
    plateID : int
        ID of the plate, for the given .ROT file.

    Returns
    -------
    array
        Two-column array on longitudes and latitudes expressed in degrees.

    """    
    
    from cartopy.io.shapereader import Reader
    import shapely
    
    reader = Reader(shapefile_path)
    
    try:
        plateContour = [plate for plate in reader.records() 
                        if plate.attributes["PLATEID1"] == plateID][0]
        
    except IndexError:
        raise IndexError("Shapefile does not contain feature with plateID = %d" %plateID)
   
    
    if isinstance(plateContour.geometry, shapely.geometry.multipolygon.MultiPolygon):
        coords_list = []
        for geom in plateContour.geometry.geoms:
            coords_list.append(np.array(geom.exterior.coords))
        
        coords_lonlat = np.vstack([np.vstack([arr,[np.nan, np.nan]]) 
                                   if i < len(coords_list) -1 else arr 
                                   for i, arr in enumerate(coords_list)])
        
    elif isinstance(plateContour.geometry, shapely.geometry.polygon.Polygon):
        coords_lonlat = np.array(plateContour.geometry.exterior.coords)
    
    else:
        raise TypeError("Not implemented shapely datatype. Must be Polygon or Multipolygon.")
        
        
    return coords_lonlat



def coordinates_to_MYRIAM(coords_lonlat, output_path):
    """
    Save a .TXT file in MYRIAM input format. The output is a two-column array 
    of coordinates expressed in longitude (deg) and latitude (deg). 

    Parameters
    ----------
    coords_lonlat : array
        Two-column array on longitudes and latitudes expressed in degrees.
    output_path : string
        Path to a .TXT file.

    Returns
    -------
    None.

    """
    
    with open(output_path, 'w+') as datafile_id: 
         np.savetxt(datafile_id, 
                    coords_lonlat,
                    comments='!',
                    fmt = ['%1.4e','%1.4e'],
                    header="lon(degE) lat(degN)",
                    )  