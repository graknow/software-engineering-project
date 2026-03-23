#Usage: randomScraper.py
import sys
from scraper import *
from scraper import recipe
import argparse

parser = argparse.ArgumentParser(
                    prog='Random Scraper',
                    description='Grabs a random recipe from TheMealDB',
                    epilog='Stop reading this, it is a waste of your time')

site = "https://www.themealdb.com/api/json/v1/1/random.php"
print(scrape(site))
