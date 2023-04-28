import subprocess
import numpy as np
import pytest


@pytest.fixture
def data():
    
    # Call test data
    arg1 = "test"
    arg2 = "tests/python"
    arg3 = "True"

    # Call the Python file with arguments using subprocess
    output = subprocess.check_output(['python', 'assets/PythonFunctions/MagnitudeHistogram.py', arg1, arg2, arg3])
    return output.decode().split()[-4:]



def test_histogram_plot(data):
    
    nBins, bar_width, bar_area, bar_height = data

    # Check that the plot has the expected number of bars
    assert int(nBins) == 50

    # Check the width of the bars
    assert np.isclose(3.4e24, float(bar_width), rtol=0, atol=1e23)

    # Check that the total area of the bars is equal to the total area of the plot
    assert np.isclose(3.4e30, float(bar_area), rtol=0, atol=1e29)

    # Check the height of the tallest bar
    assert np.isclose(71089, float(bar_height), rtol=0, atol=1e-6)