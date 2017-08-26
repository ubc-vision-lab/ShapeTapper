from PIL import Image
import glob, os,random


for infile in glob.glob("*.png"):
	file, ext = os.path.splitext(infile)
	im = Image.open(infile)
	pix =  list(im.getdata())

	for target in xrange(0,160000):
		swap = random.randint(target,160000-1)


		temp=pix[target]
		pix[target] = pix[swap]
		pix[swap] = temp

	im.putdata(pix)
		

	im.save("snow/"+file + ".png")