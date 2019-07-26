## ADR 8: Relying on a Local DB
July 26, 2019

### Context
The application runs well with calls to Panoptes for subjects and Caesar for classification counts. Although the touch table has never lost internet connection (using WiFi) and internet connection on the museum floor is quite reliable, we should be prepared for internet outages, however abrupt or short. Also, although Caesar is currently working well as a new service, we should be prepared for a situation where Caesar is unresponsive.

### Decision
We will use a local database as our source of truth in the case our external dependencies (Panoptes and Caesar) are unresponsive. The local database will be responsible for holding subject information from Panoptes and classification counts from Caesar. Furthermore, we will hold subject images locally so we will not have to wait for a potentially large array of subject images returning from the api.

We will also use a classification queue similar to the one used in PFE, whereas a new classification is kept in a queue if unable to be created through the Panoptes api (eg. in the case of lost internet connection).

### Status
Accepted

### Consequences
This will require a good amount of overhead to choose new technologies and write the logic to keep subjects locally. Also, this will introduce a new part of the app to maintain while making sure the table has the correct subjects locally as those linked to the workflow from the project builder. Things could get hairy on the table if a keen eye isn't kept on keeping the local database and subject set up to date.

My main worry is that using a local database will add another layer of complexity to the app that will make it a bit more difficult for a new developer to maintain the app. In this case, we will have to rely on strong documentation to ensure this doesn't happen.

_In Retrospect:_ Overall, I'm very satisfied with this portion of the app. The app really only needs internet connection to create a classification. Also, the app should perform well even if local subject images aren't available. The table will also keep track of classification counts correctly in the case Caesar is down. Caesar is mainly used to provide accurate classification counts in case the table gets out of sync. I strongly recommend using a local database on any future development.
