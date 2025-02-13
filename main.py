import sqlite3

# Connect to the database
conn = sqlite3.connect('database.db')
cursor = conn.cursor()

# Convert image to blob
def convert_to_blob(file_path):
    with open(file_path, 'rb') as file:
        return file.read()

# Prepare data
image_blob = convert_to_blob('../Downloads/Caesar.png')
event_data = (
    'event_002', 
    'AI Workshop', 
    image_blob, 
    'Learn about AI advancements.', 
    '2025-03-15 10:00:00', 
    '2025-03-15 16:00:00', 
    'public', 
    'open', 
    '2025-02-01 00:00:00', 
    '2025-03-14 23:59:59', 
    'auto', 
    'San Francisco', 
    'https://goo.gl/maps/example2', 
    '456 AI St, San Francisco, CA', 
    'user_002'
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