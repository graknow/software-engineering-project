#Usage: firstLetterScraper.py firstLetter
import sys
from scraper import *
from scraper import recipe
import argparse

parser = argparse.ArgumentParser(
                    prog='FirstLetter Scraper',
                    description='Grabs recipes from TheMealDB based on the first letter of the recipe',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("letter", help="The letter we are searching by")
args = parser.parse_args()

firstLetter = args.letter

site = "https://www.themealdb.com/api/json/v1/1/search.php?f=" + firstLetter
print(multiScrape(site))
