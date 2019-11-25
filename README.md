# Galaxy Zoo Touch Table
The Galaxy Zoo touch table experience at the Adler Planetarium. The Galaxy Zoo touch table is a multi-touch app built in C# and XAML with the Windows Presentation Foundation (WPF) framework. The app allows volunteers to classify galaxies according to their shape by dragging galaxy images from a central cutout in the center of the table into one of six classifiers positioned around the table. 

Please visit the [wiki](https://github.com/zooniverse/galaxy-zoo-touch-table/wiki) for more technical information. You can also view a writeup on the project through the [blog](https://blog.zooniverse.org/2019/08/14/uscientist-and-the-galaxy-zoo-touch-table-at-adler-planetarium/) (Aug 2019).

## Prerequisites
- [Local Database](#using-a-local-database)
- [Local Subjects](#using-local-subjects)

## Publishing
Currently, the app is setup to publish through a ClickOnce wizard with Google Drive File Stream, which should be setup on the local computer and synced to the Zooniverse Citizen Science team drive. Publishing can be made directly to the Touch Table folder under that drive. The Publish Version should be incremented to avoid errors with conflicting previous versions.

## Installing
Likewise, the app can be installed via the same Google Drive File Stream location (Zooniverse/Citizen Science/Touch Table/Setup). It may be necessary to uninstall older versions of the app if conflicts appear during setup.

## Log Files
Log files are placed in a system's "Documents" folder under a "TouchTable_Logs" subdirectory. This should happen in debug and release builds, although logs are only committed in debug if the screen is X'ed out, as opposed to clicking "Stop Debugging" in Visual Studio.

## Error Reporting
The app is able to automatically report crashing errors to Sentry if a valid Sentry DSN is defined in the user's system environments under the variable `SENTRY_DSN`. If no environment variable is provided, the app will still run without error reporting.

## Offline First
The app is set to run smoothly through temporary internet outages. When offline, finished classifications are placed in a queue and kept there until internet connection is established again. Internet connection is checked with each classification submitted, and queued classifications are submitted if internet is available. However, if the application is closed without internet access, all classifications held in the queue will be lost.

When offline, workflow information is loaded via a static workflow stored on the app. Subject images are collected through the local subject folder (when available) and classification counts are retrieved from the local database. However, the background space cutout in the center of the app will appear black as connection to a third party cutout service is required to retrieve the correct cutout image from a region of space.

## Using a Local Database
The app is setup to use a local database to query subject locations based on Right Ascension and Declination. In order for the app to run correctly, a SQLite database (.db) should be inserted in the system's "Documents" folder with the following paths:

- **Staging:** GZ_Staging_Subjects.db
- **Production:** GZ_Production_Subjects.db

## Using Local Subjects
In order to stay truly offline-first, the app can run with local subjects to prevent the need to fetch an image source from the internet. To do this, a "Subjects" folder must exist in the system's "Documents" folder containing paths to the local files. Within "Documents/Subjects" the app expects subfolders denoted by the first characters of a filename. Ex: The subject with a filename "J232546.75+153355.2.png" is expected to exist in "Documents/Subjects/J232/J232546.75+153355.2.png". Images should be in .png format. If a local subject is missing, the app falls back to fetching the subject online.

To create the database, a subject export (.csv) retrieved from Panoptes must be preprocessed using a [Python script](https://github.com/zooniverse/Data-digging/blob/master/example_scripts/galaxy_zoo_touch_table/prepare_db_from_classification_export.py). That .csv can then be imported into a database. I've used the [DBBrowser](https://sqlitebrowser.org/) to import CSVs.  

The local database has the following structure:

```
CREATE TABLE Subjects(
    subject_id TEXT,
    classifications_count INTEGER,
    ra REAL,
    dec REAL,
    image TEXT,
    filename TEXT,
    smooth INTEGER,
    features INTEGER,
    star INTEGER
);
```

[![pullreminders](https://pullreminders.com/badge.svg)](https://pullreminders.com?ref=badge)
