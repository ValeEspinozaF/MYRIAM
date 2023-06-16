import cartopy.crs as ccrs
import pytest

@pytest.fixture
def projection():
    return ccrs.PlateCarree()


