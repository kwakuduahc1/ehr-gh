#!/bin/bash
set -e


echo "Starting pg_restore..."

pg_restore psql -U postgres --verbose --clean --no-owner -d shims /001.sql

echo "Restore completed."
