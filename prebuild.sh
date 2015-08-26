#!/bin/bash

solution=$1
toolkitver=$2
echo Checking for .git/HEAD...
rev=nongit
if [ -e "$solution/.git/HEAD" ]
then
        echo Reading .git/HEAD...
        head=$(<"$solution/.git/HEAD")
        if [ ${head::4} == "ref:" ]
        then
                echo Reading .git/${head:5}...
                if [ -e "$solution/.git/${head:5}" ]; then commit=$(<"$solution/.git/${head:5}"); fi
        else
                commit=$head
        fi
        if [ -n "$commit" ]
        then
                echo Found commit: $commit
                rev=${commit::8}
        else
                echo Unable to find commit
        fi
fi
echo Reading ToolkitVersion.cs_dist...
origstr=00000000
cp -f "${toolkitver}_dist" "$toolkitver"
echo Writing ToolkitVersion.cs...
sed -ie "s/$origstr/$rev/g" "$toolkitver"
echo Done
#exit /b 0
