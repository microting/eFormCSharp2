#!/bin/bash
ARCH=`dpkg --print-architecture`
ARMARCH="arm64"

if [ $ARCH = $ARMARCH ]; then
	echo "WE ARE ON ARM"
	curl -SL -o https://download.visualstudio.microsoft.com/download/pr/2ea7ea69-6110-4c39-a07c-bd4df663e49b/5d60f17a167a5696e63904f7a586d072/dotnet-sdk-3.1.102-linux-arm64.tar.gz
	#curl -SL -o dotnet.tar.gz https://download.visualstudio.microsoft.com/download/pr/89fb60b1-3359-414e-94cf-359f57f37c7c/256e6dac8f44f9bad01f23f9a27b01ee/dotnet-sdk-3.0.101-linux-arm64.tar.gz
	sudo mkdir -p /usr/share/dotnet
	sudo tar -zxf dotnet.tar.gz -C /usr/share/dotnet
	sudo ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
	echo $HOME
	echo $PATH
	ls -lah /usr/share/dotnet
	#else
	#wget -q https://packages.microsoft.com/config/ubuntu/19.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
	#sudo dpkg -i packages-microsoft-prod.deb
	#sudo apt-get update
	#sudo apt-get install apt-transport-https
	#sudo apt-get update
	#sudo apt-get install -qq dotnet-sdk-3.0
	#echo "WE ARE NOT ON ARM"
	#echo $ARCH
fi
