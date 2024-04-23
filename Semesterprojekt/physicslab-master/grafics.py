import csv

import matplotlib.pyplot as plt

# Open the CSV file
with open('time_series.csv', 'r') as file:
    # Read the CSV data
    csv_data = csv.reader(file)
    
    # Get the column headers
    headers = next(csv_data)
    # Create a plot for each column
    for column in range(1,len(headers)):
        # Get the data for the current column
        data = [row[column] for row in csv_data]
        print(data)
        # Convert the data to numeric values if needed
        try:
            data = [float(value) for value in data]
        except ValueError:
            pass
        
        # Create the plot
        plt.plot(data, label=headers[column])
    
    # Add legend and show the plot
    plt.legend()
    plt.show()