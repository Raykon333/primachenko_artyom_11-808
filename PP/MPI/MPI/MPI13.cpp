#include <iostream>
#include "mpi.h"

using namespace std;
void MPI13(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	const int n = 5;
	auto matrix = new int[n][n];

	int totalCount = n * (n - 1) / 2;
	auto arr1 = new int[totalCount / size];
	auto arr2 = new int[totalCount / size];

	if (rank == 0)
	{
		srand(time(NULL));
		for (int i = 0; i < n; i++)
		{
			printf("Enter line %d\n", i);
			for (int j = 0; j < n; j++)
			{
				cin >> matrix[i][j];
			}
			cout << endl;
		}

		for (int i = 0; i < n; i++)
		{
			for (int j = 0; j < n; j++)
			{
				printf("%d\t", matrix[i][j]);
			}
			cout << endl;
		}

		int currentCount = 0, currentRank = size - 1;
		for (int i = 0; i < n - 1; i++)
		{
			for (int j = i + 1; j < n; j++)
			{
				arr1[currentCount] = matrix[i][j];
				arr2[currentCount] = matrix[j][i];
				currentCount++;
				if (currentCount == totalCount / size && currentRank != 0)
				{
					MPI_Send(&arr1[0], currentCount, MPI_INT, currentRank, 1, MPI_COMM_WORLD);
					MPI_Send(&arr2[0], currentCount, MPI_INT, currentRank, 2, MPI_COMM_WORLD);
					currentCount = 0;
					currentRank--;
					if (currentRank == 0)
					{
						arr1 = new int[totalCount / size + totalCount % size];
						arr2 = new int[totalCount / size + totalCount % size];
					}
				}
			}
		}
	}
	else
	{
		MPI_Status status;
		MPI_Recv(&arr1[0], totalCount / size, MPI_INT, 0, 1, MPI_COMM_WORLD, &status);
		MPI_Recv(&arr2[0], totalCount / size, MPI_INT, 0, 2, MPI_COMM_WORLD, &status);
	}

	int localCount = totalCount / size;
	if (rank == 0)
		localCount += totalCount % size;
	int countUnequal = 0;
	for (int i = 0; i < localCount; i++)
	{
		if (arr1[i] != arr2[i])
			countUnequal++;
	}
	printf("Process %d, unequal %d of %d\n", rank, countUnequal, localCount);
	cout << flush;

	int totalCountUnequal;
	MPI_Reduce(&countUnequal, &totalCountUnequal, 1, MPI_INT, MPI_SUM, 0, MPI_COMM_WORLD);
	if (rank == 0)
	{
		if (totalCountUnequal > 0)
			printf("Matrix isn't symmetric, unequal count = %d\n", totalCountUnequal);
		else
			printf("Matrix is symmetric\n");
	}

	MPI_Finalize();
}