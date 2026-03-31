#!/usr/bin/env python

import glob
import os
import sys
import subprocess
import argparse
import re

class functionalityHelper(argparse.Action):
    def __call__(self):
        determineFunctionality()

parser = argparse.ArgumentParser(
                    prog='Scraper Handler',
                    description='Allows for a simple call of scraper name and arguments i.e.: handler.py ingredient chicken_breast',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("-n", "--name", help="Name of the scraper to run")
parser.add_argument("-a", "--arguments", help="additional arguments, such as the ingredient being searched for")
parser.add_argument("-d", "--determine", help="Update the list of scrapers in the scrapers.txt file", action='store_true')
args = parser.parse_args()

def determineFunctionality():
    fileName = __file__
    path = fileName.replace(os.path.basename(__file__), "")
    scrapers = glob.glob(path + "*Scraper.py")

    names = open(path + "scrapers.txt", "w")
    #print(scrapers)
    firstLines = ""
    fileNames = ""
    for scraper in scrapers:
        name = re.split("Scraper.py$", scraper)
        fileNames = fileNames + name[0].split("/")[-1] + "\n"
    names.write(fileNames)

def scrape(scraperName, arguments):
    #Determine if the user has stated the available scrapers
    if not os.path.exists("./scrapers.txt"):
        determineFunctionality()

    if scraperName != "random" and arguments == None:
        print("Missing arguments for scraper")
        return

    if isinstance(arguments, str):
        systemArguements = arguments
    else:
        if scraperName == "random":
            systemArguements = ""
        else:
            systemArguements = ""
            for arguement in arguments:
                systemArguements = arguement + " "

    scraperName = os.path.join(os.path.dirname(__file__), scraperName + "Scraper.py")
    subprocess.run(["python", scraperName, systemArguements.strip()])

if args.determine:
    determineFunctionality()
else:
    scrape(args.name, args.arguments)



