echo 'Creating application user and db'

mongosh ${MONGODB_DBNAME:-alma} \
  --host localhost \
  --port ${MONGODB_PORT:-27017} \
  -u ${MONGO_INITDB_ROOT_USERNAME:-root} \
  -p ${MONGO_INITDB_ROOT_PASSWORD:-password} \
  --authenticationDatabase admin \
  --eval "db.createUser({user: '${MONGODB_USERNAME:-alma}', pwd: '${MONGODB_PASSWORD:-password}', roles: [{role: 'dbOwner', db: '${MONGODB_DBNAME:-alma}'}]});"
