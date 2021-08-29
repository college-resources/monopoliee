#!/bin/bash

UNITY_IMAGE="gableroux/unity3d:2019.3.7f1-webgl"

docker pull $UNITY_IMAGE

docker run -it --rm \
  -v "$(pwd)/../Unity:/root/project" \
  -e HOST_UID=$UID \
  $UNITY_IMAGE \
  bash /root/project/docker/build.sh

mv ../Unity/Build ../Backend/unity-build

docker build -t cores/monopoliee -f ./Dockerfile ../Backend

docker-compose up -d
