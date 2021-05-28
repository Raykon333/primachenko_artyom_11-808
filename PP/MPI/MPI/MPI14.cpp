#include <iostream>
#include "mpi.h"

using namespace std;
void MPI14(int argc, char** argv)
{
	int rank, size;
	MPI_Init(&argc, &argv);
	MPI_Comm_size(MPI_COMM_WORLD, &size);
	MPI_Comm_rank(MPI_COMM_WORLD, &rank);

	
	const int n = 21;
	int arr[n];
	if (rank == 0)
	{
		srand(time(NULL));
		for (int i = 0; i < n; i++)
		{
			arr[i] = rand() % 100;
			printf("%d ", arr[i]);
		}
		cout << endl;
	}

	int totalStepSwaps;
	int step = 0;
	do
	{
		totalStepSwaps = 0;
		int totalPairs = n / 2 - step % 2 + (n % 2) * (step%2);

		auto sendCounts = new int[size];
		auto disps = new int[size];
		disps[0] = 0;
		for (int i = 0; i < size; i++)
		{
			sendCounts[i] = totalPairs / size * 2;
			if (i == size - 1)
				sendCounts[i] += totalPairs % size * 2;
			if (i != 0)
				disps[i] = disps[i - 1] + sendCounts[i - 1];
		}
		auto localArr = new int[sendCounts[rank]];
		MPI_Scatterv(&arr[step % 2], sendCounts, disps, MPI_INT, &localArr[0], sendCounts[rank], MPI_INT, 0, MPI_COMM_WORLD);

		/*printf("Process %d, array ", rank);
		for (int i = 0; i < sendCounts[rank]; i++)
			printf("%d ", localArr[i]);
		cout << endl;*/

		int localStepSwaps = 0;
		for (int i = 0; i < sendCounts[rank]; i += 2)
		{
			if (localArr[i] > localArr[i + 1])
			{
				swap(localArr[i], localArr[i + 1]);
				localStepSwaps++;
			}
		}

		/*printf("After process %d, array ", rank);
		for (int i = 0; i < sendCounts[rank]; i++)
			printf("%d ", localArr[i]);
		cout << endl;*/

		MPI_Gatherv(&localArr[0], sendCounts[rank], MPI_INT, &arr[step % 2], sendCounts, disps, MPI_INT, 0, MPI_COMM_WORLD);
		MPI_Reduce(&localStepSwaps, &totalStepSwaps, 1, MPI_INT, MPI_SUM, 0, MPI_COMM_WORLD);

		if (rank == 0)
		{
			for (int i = 0; i < n; i++)
			{
				printf("%d ", arr[i]);
			}
			cout << endl;
			printf("swaps = %d\n", totalStepSwaps);
		}

		MPI_Bcast(&totalStepSwaps, 1, MPI_INT, 0, MPI_COMM_WORLD);
		step++;
	} while (totalStepSwaps > 0);

	MPI_Finalize();
}