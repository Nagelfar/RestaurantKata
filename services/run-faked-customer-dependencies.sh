#! /bin/bash
docker-compose up --build GuestExperience TableService Billing
docker-compose down -v