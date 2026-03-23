
import glob
import os
import sys
import subprocess
import argparse

def determineFunctionality():
    scrapers = glob.glob("./*Scraper.py")
    names = open("./scrapers.txt", "w")
    firstLines = ""
    for scraper in scrapers:
        with open(scraper, "r") as file:
            firstline = file.readline().strip()
            if("Usage:" in firstline):
                print(firstline.split("Usage: ")[1])
                firstLines = firstLines + firstline + "\n"

    names.write(firstLines)


def scrape(scraperName, arguments):
    #Determine if the user has stated the available scrapers
    if not os.path.exists("./scrapers.txt"):
        determineFunctionality()

    systemArguements = ""
    for arguement in arguments:
        systemArguements = arguement + " "

    scraperName = scraperName + "Scraper.py"
    subprocess.run(["python", scraperName, systemArguements.strip()])

arguments = []
for arg in sys.argv:
    arguments.append(arg)
scrape(sys.argv[1], arguments)



parser = argparse.ArgumentParser(
                    prog='Scraper Handler',
                    description='',
                    epilog='Text at the bottom of help')
