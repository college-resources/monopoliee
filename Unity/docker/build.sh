#!/bin/bash

if [ ! -f /root/project/docker/Unity_v2019.x.ulf ]; then
    cp /Unity_*.alf /root/project/docker/
    chmod 666 /root/project/docker/Unity_*.alf
    read -p 'Please copy Unity_v2019.x.ulf to the Unity/docker folder and press enter.'
fi

sed -i 's/\/\/ #define MONOPOLIEE_PRODUCTION_MODE/#define MONOPOLIEE_PRODUCTION_MODE/' /root/project/Assets/Scripts/ApiWrapper.cs

/opt/Unity/Editor/Unity \
    -nographics \
    -batchmode \
    -logfile /dev/stdout \
    -force-opengl \
    -quit \
    -manualLicenseFile /root/project/docker/Unity_v2019.x.ulf

/opt/Unity/Editor/Unity \
    -nographics \
    -batchmode \
    -logfile /dev/stdout \
    -force-opengl \
    -quit \
    -projectPath /root/project \
    -executeMethod WebGLBuilder.build

chown -R $HOST_UID:$HOST_UID /root/project/
