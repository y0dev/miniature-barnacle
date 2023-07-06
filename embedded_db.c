#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#define MAX_SIZE 100

typedef struct
{
    int id;
    char name[20];
    char email[50];
} Record;

typedef struct
{
    Record *records[MAX_SIZE];
} Database;

int hashFunction(int id)
{
    return id % MAX_SIZE;
}

void insertRecord(Database *db, Record *record)
{
    int index = hashFunction(record->id);
    db->records[index] = record;
}

Record *searchRecord(Database *db, int id)
{
    int index = hashFunction(id);
    return db->records[index];
}

void writeDatabaseToFile(Database *db, const char *filename)
{
    FILE *file = fopen(filename, "wb");
    if (file == NULL)
    {
        printf("Failed to open file for writing.\n");
        return;
    }

    for (int i = 0; i < MAX_SIZE; i++)
    {
        if (db->records[i] != NULL)
        {
            fwrite(db->records[i], sizeof(Record), 1, file);
        }
    }

    fclose(file);
}

void readDatabaseFromFile(Database *db, const char *filename)
{
    FILE *file = fopen(filename, "rb");
    if (file == NULL)
    {
        printf("Failed to open file for reading.\n");
        return;
    }

    Record record;
    while (fread(&record, sizeof(Record), 1, file) == 1)
    {
        printf("Record: ID=0X%02X Name=%s Email=%s\n", record.id, record.name, record.email);
        insertRecord(db, &record);
    }

    fclose(file);
}

int main()
{
    Database db;
    memset(&db, 0, sizeof(Database));

    // Create and insert records
    Record record1 = {1, "John", "john@example.com"};
    Record record2 = {2, "Jane", "jane@example.com"};
    Record record3 = {3, "Bob", "bob@example.com"};

    insertRecord(&db, &record1);
    insertRecord(&db, &record2);
    insertRecord(&db, &record3);

    // Write database to a binary file
    const char *filename = "database_c.bin";
    writeDatabaseToFile(&db, filename);
    printf("Database written to file: %s\n", filename);

    // Clear the database
    memset(&db, 0, sizeof(Database));

    // Read database from the binary file
    readDatabaseFromFile(&db, filename);
    printf("Database loaded from file.\n");

    // Search for a record by ID
    int searchId = 2;
    Record *result = searchRecord(&db, searchId);
    if (result != NULL)
    {
        printf("Record found: ID=%d, Name=%s, Email=%s\n", result->id, result->name, result->email);
    }
    else
    {
        printf("Record with ID %d not found.\n", searchId);
    }

    return 0;
}
