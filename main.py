import struct

class Record:
    def __init__(self, id, name, email):
        self.id = id
        self.name = name
        self.email = email

def write_database_to_file(database, filename):
    with open(filename, "wb") as file:
        for record in database:
            data = struct.pack("i20s50s", record.id, record.name.encode("utf-8"), record.email.encode("utf-8"))
            file.write(data)

def read_database_from_file(filename):
    database = []
    with open(filename, "rb") as file:
        while True:
            data = file.read(struct.calcsize("i20s50s"))
            if not data:
                break
            record_data = struct.unpack("i20s50s", data)
            id = record_data[0]
            name = record_data[1].decode("utf-8").rstrip("\x00")
            email = record_data[2].decode("utf-8").rstrip("\x00")
            record = Record(id, name, email)
            database.append(record)
    return database

# Example usage

# Create a list of records
database = [
    Record(1, "John", "john@example.com"),
    Record(2, "Jane", "jane@example.com"),
    Record(3, "Bob", "bob@example.com")
]

# Write the database to a binary file
filename = "database.bin"
write_database_to_file(database, filename)
print("Database written to file:", filename)

# Clear the database
database = []

# Read the database from the binary file
database = read_database_from_file(filename)
print("Database loaded from file:")

# Print the records
for record in database:
    print("ID:", record.id)
    print("Name:", record.name)
    print("Email:", record.email)
    print()
