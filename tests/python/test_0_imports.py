import pytest

## ==========================

def test_numpy_import():
    import numpy
    print("\t\t You have numpy version {}".format(numpy.__version__))  
    
    
def test_pandas_import():
    import pandas
    print("\t\t You have pandas version {}".format(pandas.__version__))
    
    
def test_matplotlib_import():
    import matplotlib
    print("\t\t You have pandas version {}".format(matplotlib.__version__))


def test_cartopy_import():
    import cartopy
    print("\t\t You have cartopy version {}".format(cartopy.__version__))
