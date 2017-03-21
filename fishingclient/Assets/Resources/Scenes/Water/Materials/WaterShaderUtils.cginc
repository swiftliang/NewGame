
		float CalculateWave(float u, float fAmplitude, float fPhase1, float fPhase2)
		{
			/// Big
			float waveTime = _Time.x * fPhase1;
			float animTime =  u + waveTime;		 						
			float wave = cos(1.5 * animTime) * 1.8;

			/// Small
			float waveTime2 = _Time.z * fPhase2;
			float animTime2 = u + waveTime2;
			float wave2 = cos(5 * animTime2);

			return (wave + wave2) * fAmplitude;
		}

		float CalculateBigWave(float phrase, float waveTime, float waveOffset, float amplitude) 
		{
			/// Calculate big Wave
			float animTime = phrase + waveTime;
			float fAmplitude = cos(36 * waveTime);
			float waveRadius = 2.0;
			float fFreqFadeByDistance = clamp( abs(phrase - waveOffset), 0, waveRadius);
			float fFadeByDistance = waveRadius - fFreqFadeByDistance;
			fFadeByDistance = pow(fFadeByDistance, 3);
			fFreqFadeByDistance = pow(fFreqFadeByDistance, 2.5);
			float bigWave =  fFadeByDistance * amplitude * fAmplitude * cos(fFreqFadeByDistance * 5.43);

			return bigWave;
		}



