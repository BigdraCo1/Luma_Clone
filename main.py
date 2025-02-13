import sqlite3
import random
import uuid
from datetime import datetime, timedelta

# Connect to the database
conn = sqlite3.connect('database.db')
cursor = conn.cursor()

# Convert image to blob
def convert_to_blob(file_path):
    with open(file_path, 'rb') as file:
        return file.read()

# Generate random date within a range
def random_date(start, end):
    return start + timedelta(
        seconds=random.randint(0, int((end - start).total_seconds())),
    )

# List of random locations
locations = [
    'Bangkok',
    'San Francisco',
    'New York',
    'London',
    'Tokyo',
    'Sydney'
]

# Prepare image blobs
images_blob = [convert_to_blob(f'../Downloads/mock_event/{i}.png') for i in range(1, 7)]

# Insert each event with a different image, random date, and location
for i, image_blob in enumerate(images_blob, start=1):
    start_date = random_date(datetime(2025, 3, 1), datetime(2025, 3, 31))
    end_date = start_date + timedelta(hours=6)
    location = random.choice(locations)
    
    event_data = (
        str(uuid.uuid4()),  # Generate a unique TUID
        f'Event {i}', 
        image_blob, 
        f'Description for event {i}', 
        start_date.strftime('%Y-%m-%d %H:%M:%S'), 
        end_date.strftime('%Y-%m-%d %H:%M:%S'), 
        'public', 
        'open', 
        '2025-02-01 00:00:00', 
        '2025-03-14 23:59:59', 
        'auto', 
        location, 
        'https://goo.gl/maps/example2', 
        '456 AI St, San Francisco, CA', 
        f'user_{i:03}'
    )

    # Insert into Event table
    cursor.execute("""
        INSERT INTO Event (
            TUID, 
            name, 
            image, 
            description, 
            created, 
            start, 
            end, 
            publicity, 
            registrationStatus, 
            registrationStart, 
            registrationEnd, 
            approvalType, 
            locationCity, 
            locationGmapUrl, 
            location, 
            ownerTUID
        ) VALUES (?, ?, ?, ?, DATETIME('now'), ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
    """, event_data)

# Commit changes and close
conn.commit()
conn.close()