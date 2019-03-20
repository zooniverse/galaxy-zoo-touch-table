# Galaxy Zoo Touch Table
The Galaxy Zoo touch table experience at the Adler Planetarium.

## Prerequisites
- [Local Database](#using-a-local-database)

## Publishing
Currently, the app is setup to publish through a ClickOnce wizard with Google Drive File Stream, which should be setup on the local computer and synced to the Zooniverse Citizen Science team drive. Publishing can be made directly to the Touch Table folder under that drive. The Publish Version should be incremented to avoid errors with conflicting previous versions.

## Installing
Likewise, the app can be installed via the same Google Drive File Stream location (Zooniverse/Citizen Science/Touch Table/Setup). It may be necessary to uninstall older versions of the app if conflicts appear during setup.

## Using a Local Database
The touch table is setup to use a local database to query subject locations based on Right Ascension and Declination. In order for the app to run correctly, a SQLite database (.db) should be inserted in the system's "Documents" folder with the following paths:

- **Staging:** GZ_Staging_Subjects.db
- **Production:** GZ_Production_Subjects.db

To create the database, a subject export (.csv) retrieved from Panoptes must be preprocessed using a Python script. That .csv can then be imported into a database. I've used the [SQLiteStudio GUI](https://sqlitestudio.pl/index.rvt) to import CSVs.  

The local database has the following structure:

```
CREATE TABLE Subjects(
    subject_id STRING,
    classification_count INTEGER,
    ra DOUBLE,
    dec DOUBLE,
    image STRING
);
```

[![pullreminders](https://pullreminders.com/badge.svg)](https://pullreminders.com?ref=badge)
