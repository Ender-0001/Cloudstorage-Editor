#include "program.h"
#include "build.h"
#include "util.h"

namespace aurora {
	Program::Program() {
		Util::InitConsole();
#ifdef _PROD
#endif

		m_pCurl = new Curl;
	}
	Program::~Program() {
		if (m_pCurl) {
			delete m_pCurl;
		}
	}
}
