echo "tailwindcss-extra is not installed, attempting install ... "

curl -o tailwindcss-extra https://github.com/dobicinaitis/tailwind-cli-extra/releases/download/v2.9.6/tailwindcss-extra-linux-x64 
chmod +x tailwindcss-extra;

# moving it to tools for ease of use.
mv tailwindcss-extra ~/.dotnet/tools/

# add alias for tw in .bashrc
echo "alias tailwindcss-extra='~/.dotnet/tools/tailwindcss-extra $1'" >> ~/.bashrc
source ~/.bashrc

