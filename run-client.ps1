docker run -it `
  -e AppConfig__AppType=client `
  -e AppConfig__IpAddress=127.0.0.1 `
  -e AppConfig__Port=8080 `
  -e AppConfig__Username=testuser `
  -e AppConfig__Password=secret `
  -e AppConfig__Communicator=UdpCommunicator `
  -e AppConfig__LogLevel=Info `
  -e AppConfig__AppName=CorePunkServerSamples `
  corepunkserversamples-corepunkclient
