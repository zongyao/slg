
#include <iostream>
using namespace std;

void Swap(char *charArray, int index1, int index2)
{
	if (index1 == index2)
	{
		return;
	}
	char temp = charArray[index2]; 
	charArray[index2] = charArray[index1];
	charArray[index1] = temp;
}

void ReverseCharArry(char *charArray)
{
	int length = strlen(charArray);
	for (int i = 0; i < length / 2; i++)
	{
		Swap(charArray, i, length - i - 1);
	}
}

void DeleSapce(char *charArray)
{
	char *tempCharArray = charArray;
	while (*tempCharArray == ' ') {
		tempCharArray++;
	}
	int j = 0;
	while (*tempCharArray != '\0')
	{
		charArray[j++] = *tempCharArray++;
		while (charArray[j - 1] == ' '&& *tempCharArray == ' ')
		{
			tempCharArray++;
		}
	}

	if (charArray[j - 1] == ' ')
	{
		charArray[j - 1] = '\0';
	}
	else
	{
		charArray[j] = '\0';
	}
}
void  ReverseItem(char *charArray)
{
	int length = strlen(charArray);
	int swapStart = 0;

	for (int j = 0; j <length; j++)
	{
		if (j == 0)
		{
			if (charArray[j] == ' ')
			{
				swapStart = 1;
			}
		}
		else if (charArray[j] == ' ')
		{
			int swapEnd = j - 1;
			for (int swapIndex = swapStart; swapIndex <= swapEnd; swapIndex++, swapEnd--)
			{

				if (swapEnd > swapIndex)
				{
					Swap(charArray, swapIndex, swapEnd);
				}
			}
			swapStart = j + 1;
		}
	}
}

void Display(char *charArray)
{
	int length = strlen(charArray);
	for (int i = 0; i < length ; i++)
	{
		cout << charArray[i] ;
	}
}
int main()
{
	
	char charArray[] = "     1       2 3   45   6    99   ";
	DeleSapce(charArray);
	ReverseCharArry(charArray);
	ReverseItem(charArray);
	Display(charArray);

	system("pause");
	return 0;
}


