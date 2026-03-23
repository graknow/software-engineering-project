
import glob
import os
import sys
import subprocess
import argparse

parser = argparse.ArgumentParser(
                    prog='Scraper Handler',
                    description='Allows for a simple call of scraper name and arguments i.e.: handler.py ingredient chicken_breast',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("name", help="Name of the scraper to run")
parser.add_argument("-a", "--arguments", help="additional arguments, such as the ingredient being searched for")
args = parser.parse_args()

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

    scraperName = scraperName + "Scraper.py"
    subprocess.run(["python", scraperName, systemArguements.strip()])

scrape(args.name, args.arguments)



