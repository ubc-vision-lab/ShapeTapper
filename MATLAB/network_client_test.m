UnityPort = tcpip('localhost', 56790);
set(UnityPort, 'InputBufferSize', 5000);

fprintf('connecting to client\n');
fopen(UnityPort);
fprintf('client connected\n');
fprintf(UnityPort,"Hello Unity!");

pause(1);

while(UnityPort.BytesAvailable <= 0)
end

while(get(UnityPort,'BytesAvailable')>0)
    A = fscanf(UnityPort);
end

fclose(UnityPort);
delete(UnityPort);
clear UnityPort